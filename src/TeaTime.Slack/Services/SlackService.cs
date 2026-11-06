namespace TeaTime.Slack.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Common.Abstractions;
using Common.Collections;
using Common.Exceptions;
using Common.Features.Links.Commands;
using Common.Features.Links.Queries;
using Common.Features.Options.Queries;
using Common.Features.Orders.Commands;
using Common.Features.Orders.Queries;
using Common.Features.RoomItemGroups.Queries;
using Common.Features.Rooms.Commands;
using Common.Features.Rooms.Queries;
using Common.Features.Runs.Queries;
using Common.Features.Users.Commands;
using Common.Features.Users.Queries;
using Common.Models;
using Common.Models.Data;
using Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Models;
using Models.Interactivity;
using Models.OAuth;
using Models.SlashCommands;
using Resources;

internal class SlackService(ISender mediator,
    IIdGenerator<long> idGenerator,
    IDistributedHash hash,
    TimeProvider timeProvider,
    ISlackApiClient slackApiClient,
    ILogger<SlackService> logger)
    : ISlackService
{
    public async Task<User> GetOrCreateUserAsync(string userId, string? name,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var id = await mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.User, userId), cancellationToken);
        if (id > 0)
        {
            return await mediator.Send(new GetUserQuery(id.Value), cancellationToken) ??
                   throw new InvalidOperationException($"Failed to get user {id.Value}");
        }

        //we need to create a new user
        var command = new CreateUserCommand
        (
            Id: await idGenerator.GenerateAsync(cancellationToken),
            Username: "slack_" + userId,
            // Slack may not send a name; never store a bare ID as one
            DisplayName: string.IsNullOrWhiteSpace(name) ? FallbackUserName(userId) : name
        );
        await mediator.Send(command, cancellationToken);

        //add link
        await mediator.Send(new CreateLinkCommand(command.Id, LinkType.User, userId), cancellationToken);

        //this is not great because its not really a proper entity model, but it will do for now
        //we shouldn't query here because the command COULD eventually be eventual consistency
        return new User
        {
            Id = command.Id,
            DisplayName = command.DisplayName,
            Username = command.Username,
            CreatedDate = DateTimeOffset.MinValue //dunno this value *shrug*
        };
    }

    public async Task<Room> GetOrCreateRoomAsync(string channelId, string? channelName, long userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channelId);

        if (userId == default)
            throw new ArgumentException("value cannot be empty", nameof(userId));

        var roomId = await mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.Room, channelId), cancellationToken);
        if (roomId > 0)
        {
            return await mediator.Send(new GetRoomQuery(roomId.Value), cancellationToken) ??
                   throw new InvalidOperationException($"Failed to get room {roomId.Value}");
        }

        var command = new CreateRoomCommand
        (
            Id: await idGenerator.GenerateAsync(cancellationToken),
            Name: string.IsNullOrWhiteSpace(channelName) ? FallbackChannelName(channelId) : channelName,
            UserId: userId
        );

        await mediator.Send(command, cancellationToken);

        //add link
        await mediator.Send(new CreateLinkCommand(command.Id, LinkType.Room, channelId), cancellationToken);

        return new Room
        {
            Id = command.Id,
            Name = command.Name,
            CreatedBy = userId,
            CreatedDate = DateTimeOffset.MinValue //dunno this value *shrug*
        };
    }

    public async Task JoinRunAsync(SlashCommand slashCommand, string optionName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(slashCommand);
        ArgumentNullException.ThrowIfNull(optionName);

        var user = await GetOrCreateUserAsync(slashCommand.UserId, slashCommand.GetUserName(), cancellationToken);
        var room = await GetOrCreateRoomAsync(slashCommand.ChannelId, slashCommand.ChannelName, user.Id,
            cancellationToken);

        await JoinRunAsync(user.Id, room.Id, optionName, slashCommand.ToCallbackData(), cancellationToken);
    }

    public async Task LeaveRunAsync(SlashCommand slashCommand, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(slashCommand);

        var user = await GetOrCreateUserAsync(slashCommand.UserId, slashCommand.GetUserName(), cancellationToken);
        var room = await GetOrCreateRoomAsync(slashCommand.ChannelId, slashCommand.ChannelName, user.Id,
            cancellationToken);

        await LeaveRunAsync(user.Id, room.Id, null, slashCommand.ToCallbackData(), cancellationToken);
    }

    public async Task HandleBlockActionsAsync(BlockActionsPayload requestPayload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestPayload);

        // Interaction acks can't carry a message, so failures are reported via response_url
        if (string.IsNullOrWhiteSpace(requestPayload.ResponseUrl))
        {
            logger.LogWarning("Ignoring block_actions payload without response_url");
            return;
        }

        var action = requestPayload.Actions.FirstOrDefault();

        try
        {
            if (action is null)
                throw new InvalidOperationException("block_actions payload has no actions");

            if (action.ActionId == BlockBuilder.LeaveRunActionId)
                await LeaveRunAsync(requestPayload, action, requestPayload.ResponseUrl, cancellationToken);
            else if (IsJoinAction(action))
                await JoinRunAsync(requestPayload, action, requestPayload.ResponseUrl, cancellationToken);
            else
                throw new InvalidOperationException($"Unknown block action '{action.ActionId}'");
        }
        catch (SlackTeaTimeException e)
        {
            logger.LogInformation(e, "Block action {ActionId} rejected", action?.ActionId);
            await ReplyAsync(requestPayload.ResponseUrl, e.Message, cancellationToken);
        }
        catch (PermissionException e)
        {
            logger.LogInformation(e, "Block action {ActionId} denied", action?.ActionId);
            await ReplyAsync(requestPayload.ResponseUrl, e.Message, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to handle block action {ActionId}", action?.ActionId);
            await ReplyAsync(requestPayload.ResponseUrl, ErrorStrings.General(), cancellationToken);
        }
    }

    public async Task InstallAsync(OAuthTokenResponse response, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);

        // Org-wide installs have no team
        if (response.IsEnterpriseInstall || response.Team is null)
            throw new InvalidOperationException("Org-wide installs are not supported");

        if (string.IsNullOrEmpty(response.Team.Id))
            throw new InvalidOperationException("TeamId cannot be empty");

        if (string.IsNullOrEmpty(response.AccessToken))
            throw new InvalidOperationException("AccessToken cannot be empty");

        var hashKey = "slack:" + response.Team.Id;

        // Store base data for the whole team
        List<HashEntry> entries =
        [
            new(Constants.FieldKeys.AccessToken, response.AccessToken),
            new(Constants.FieldKeys.InstallTime, timeProvider.GetUtcNow().ToString("O"))
        ];

        if (!string.IsNullOrEmpty(response.Team.Name))
            entries.Add(new(Constants.FieldKeys.TeamName, response.Team.Name));

        await hash.SetAsync(hashKey, entries, cancellationToken);
    }

    private static bool IsJoinAction(BlockAction action) =>
        action.ActionId.StartsWith(BlockBuilder.JoinOptionActionIdPrefix, StringComparison.Ordinal);

    private static string FallbackUserName(string slackUserId) => $"Slack User {slackUserId}";

    private static string FallbackChannelName(string slackChannelId) => $"Slack Channel {slackChannelId}";

    private Task ReplyAsync(string responseUrl, string message, CancellationToken cancellationToken = default)
    {
        return slackApiClient.PostResponseAsync(responseUrl, new SlashCommandResponse(message, ResponseType.User),
            cancellationToken);
    }

    private async Task JoinRunAsync(BlockActionsPayload requestPayload, BlockAction action, string responseUrl,
        CancellationToken cancellationToken = default)
    {
        if (!BlockBuilder.TryParseJoinValue(action.Value, out var runId, out var optionId))
            throw new InvalidOperationException($"Invalid join value '{action.Value}'");

        var (user, room) = await GetUserAndRoomAsync(requestPayload, cancellationToken);
        var callbackData = requestPayload.ToCallbackData(responseUrl);

        await JoinRunAsync(user.Id, room.Id, runId, optionId, callbackData, cancellationToken);
    }

    private async Task LeaveRunAsync(BlockActionsPayload requestPayload, BlockAction action, string responseUrl,
        CancellationToken cancellationToken = default)
    {
        if (!long.TryParse(action.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var runId))
            throw new InvalidOperationException($"Invalid run id '{action.Value}'");

        var (user, room) = await GetUserAndRoomAsync(requestPayload, cancellationToken);

        await LeaveRunAsync(user.Id, room.Id, runId, requestPayload.ToCallbackData(responseUrl), cancellationToken);
    }

    private async Task<(User User, Room Room)> GetUserAndRoomAsync(BlockActionsPayload requestPayload,
        CancellationToken cancellationToken = default)
    {
        var channel = requestPayload.Channel;
        if (channel is null)
            throw new InvalidOperationException("block_actions payload has no channel");

        var usersName = requestPayload.User.Username ?? requestPayload.User.Name;

        var user = await GetOrCreateUserAsync(requestPayload.User.Id, usersName, cancellationToken);
        var room = await GetOrCreateRoomAsync(channel.Id, channel.Name, user.Id, cancellationToken);

        return (user, room);
    }

    private async Task LeaveRunAsync(long userId, long roomId, long? runId, CallbackData callbackData,
        CancellationToken cancellationToken = default)
    {
        var run = await GetRunAsync(userId, roomId, runId, cancellationToken);
        if (run == null)
            throw new SlackTeaTimeException(ErrorStrings.LeaveRun_RunNotStarted());

        // Checked before the order lookup so an ended round replies ended rather than not joined
        if (run.Ended)
            throw new SlackTeaTimeException(ErrorStrings.LeaveRun_RunEnded());

        var existingOrder = await mediator.Send(new GetUserOrderQuery(run.Id, userId), cancellationToken);
        if (existingOrder is null)
            throw new SlackTeaTimeException(ErrorStrings.LeaveRun_NotJoined());

        var command = new DeleteOrderCommand(existingOrder, userId);

        command.AddCallbackState(callbackData);

        try
        {
            await mediator.Send(command, cancellationToken);
        }
        catch (OrderException e) when (e.Reason == OrderException.OrderExceptionReason.RunEnded)
        {
            throw new SlackTeaTimeException(ErrorStrings.LeaveRun_RunEnded(), e);
        }
    }

    private async Task JoinRunAsync(long userId, long roomId, string optionName, CallbackData callbackData,
        CancellationToken cancellationToken = default)
    {
        var run = await mediator.Send(new GetCurrentRunQuery(roomId, userId), cancellationToken);
        if (run == null)
            throw new SlackTeaTimeException(ErrorStrings.JoinRun_RunNotStarted());

        var group = await mediator.Send(new GetRoomItemGroupQuery(
                RoomId: run.RoomId,
                UserId: userId,
                GroupId: run.GroupId),
            cancellationToken);

        if (group == null)
        {
            logger.LogWarning("Failed to find group {GroupId} in room {RoomId} for run {RunId}",
                run.GroupId, roomId, run.Id);

            throw new SlackTeaTimeException(ErrorStrings.General());
        }

        var option = group.Options
            .FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));

        if (option == null)
            throw new SlackTeaTimeException(ErrorStrings.OptionUnknown());

        await JoinRunInternalAsync(userId, run, option, callbackData, cancellationToken);
    }

    private async Task JoinRunAsync(long userId, long roomId, long runId, long optionId, CallbackData callbackData,
        CancellationToken cancellationToken = default)
    {
        var run = await GetRunAsync(userId, roomId, runId, cancellationToken);
        if (run == null)
            throw new SlackTeaTimeException(ErrorStrings.JoinRun_RunNotStarted());

        // Checked before the option lookup so an ended round replies ended rather than unknown option
        if (run.Ended)
            throw new SlackTeaTimeException(ErrorStrings.JoinRun_RunEnded());

        var option = await mediator.Send(new GetOptionQuery(optionId), cancellationToken);
        if (option == null || option.GroupId != run.GroupId)
            throw new SlackTeaTimeException(ErrorStrings.OptionUnknown());

        await JoinRunInternalAsync(userId, run, option, callbackData, cancellationToken);
    }

    private async Task JoinRunInternalAsync(long userId, Run run, Option option, CallbackData callbackData,
        CancellationToken cancellationToken = default)
    {
        BaseCommand command;

        //check if we need to join or update
        var existingOrder = await mediator.Send(new GetUserOrderQuery(run.Id, userId), cancellationToken);
        if (existingOrder == null)
        {
            command = new CreateOrderCommand
            (
                Id: await idGenerator.GenerateAsync(cancellationToken),
                RunId: run.Id,
                UserId: userId,
                OptionId: option.Id
            );
        }
        else
        {
            //we need to update the existing order
            command = new UpdateOrderOptionCommand(existingOrder, userId, option.Id);
        }

        command.AddCallbackState(callbackData);

        try
        {
            await mediator.Send(command, cancellationToken);
        }
        catch (OrderException e) when (e.Reason == OrderException.OrderExceptionReason.OptionUnavailable)
        {
            throw new SlackTeaTimeException(ErrorStrings.JoinRun_OptionUnavailable(option.Name), e);
        }
        catch (OrderException e) when (e.Reason == OrderException.OrderExceptionReason.RunEnded)
        {
            throw new SlackTeaTimeException(ErrorStrings.JoinRun_RunEnded(), e);
        }
    }

    private async Task<Run?> GetRunAsync(long userId, long roomId, long? runId,
        CancellationToken cancellationToken = default)
    {
        // Slash commands act on the current run, buttons act on the run their message was posted for
        if (!runId.HasValue)
            return await mediator.Send(new GetCurrentRunQuery(roomId, userId), cancellationToken);

        var run = await mediator.Send(new GetRunQuery(runId.Value), cancellationToken);

        // A run id from another room's message is treated as no run
        if (run == null || run.RoomId != roomId)
            return null;

        return run;
    }
}
