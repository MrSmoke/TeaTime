namespace TeaTime.Slack.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
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
        public async Task<User> GetOrCreateUser(string userId, string? name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            var id = await mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.User, userId));
            if (id > 0)
            {
                return await mediator.Send(new GetUserQuery(id.Value)) ??
                       throw new InvalidOperationException($"Failed to get user {id.Value}");
            }

            //we need to create a new user
            var command = new CreateUserCommand
            (
                Id: await idGenerator.GenerateAsync(),
                Username: "slack_" + userId,
                // Slack may not send a name; never store a bare ID as one
                DisplayName: string.IsNullOrWhiteSpace(name) ? FallbackUserName(userId) : name
            );
            await mediator.Send(command);

            //add link
            await mediator.Send(new CreateLinkCommand(command.Id, LinkType.User, userId));

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

        public async Task<Room> GetOrCreateRoom(string channelId, string? channelName, long userId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(channelId);

            if (userId == default)
                throw new ArgumentException("value cannot be empty", nameof(userId));

            var roomId = await mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.Room, channelId));
            if (roomId > 0)
            {
                return await mediator.Send(new GetRoomQuery(roomId.Value)) ??
                       throw new InvalidOperationException($"Failed to get room {roomId.Value}");
            }

            var command = new CreateRoomCommand
            (
                Id: await idGenerator.GenerateAsync(),
                Name: string.IsNullOrWhiteSpace(channelName) ? FallbackChannelName(channelId) : channelName,
                UserId: userId
            );

            await mediator.Send(command);

            //add link
            await mediator.Send(new CreateLinkCommand(command.Id, LinkType.Room, channelId));

            return new Room
            {
                Id = command.Id,
                Name = command.Name,
                CreatedBy = userId,
                CreatedDate = DateTimeOffset.MinValue //dunno this value *shrug*
            };
        }

        public async Task JoinRunAsync(SlashCommand slashCommand, string optionName)
        {
            ArgumentNullException.ThrowIfNull(slashCommand);
            ArgumentNullException.ThrowIfNull(optionName);

#pragma warning disable CS0618 // Read while Slack still sends it; GetOrCreateUser falls back when missing
            var user = await GetOrCreateUser(slashCommand.UserId, slashCommand.UserName);
#pragma warning restore CS0618
            var room = await GetOrCreateRoom(slashCommand.ChannelId, slashCommand.ChannelName, user.Id);

            await JoinRunAsync(user.Id, room.Id, optionName, slashCommand.ToCallbackData());
        }

        public async Task HandleBlockActionsAsync(BlockActionsPayload requestPayload)
        {
            ArgumentNullException.ThrowIfNull(requestPayload);

            // Interaction acks can't carry a message, so failures are reported via response_url
            if (string.IsNullOrWhiteSpace(requestPayload.ResponseUrl))
            {
                logger.LogWarning("Ignoring block_actions payload without response_url");
                return;
            }

            try
            {
                await JoinRunAsync(requestPayload);
            }
            catch (SlackTeaTimeException e)
            {
                logger.LogWarning("Join run rejected: {Message}", e.Message);
                await ReplyAsync(requestPayload.ResponseUrl, e.Message);
            }
            catch (PermissionException e)
            {
                logger.LogWarning("Join run denied: {Message}", e.Message);
                await ReplyAsync(requestPayload.ResponseUrl, e.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to join run");
                await ReplyAsync(requestPayload.ResponseUrl, ErrorStrings.General());
            }
        }

        private Task ReplyAsync(string responseUrl, string message)
        {
            return slackApiClient.PostResponseAsync(responseUrl, new SlashCommandResponse(message, ResponseType.User));
        }

        private async Task JoinRunAsync(BlockActionsPayload requestPayload)
        {
            var channel = requestPayload.Channel;
            if (channel is null)
                throw new InvalidOperationException("block_actions payload has no channel");

            var action = requestPayload.Actions.FirstOrDefault(IsJoinAction);
            if (action is null)
                throw new InvalidOperationException("block_actions payload has no join action");

            if (!long.TryParse(action.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var optionId))
                throw new InvalidOperationException($"Invalid option id '{action.Value}'");

            var usersName = requestPayload.User.Username ?? requestPayload.User.Name;

            var user = await GetOrCreateUser(requestPayload.User.Id, usersName);
            var room = await GetOrCreateRoom(channel.Id, channel.Name, user.Id);

            await JoinRunAsync(user.Id, room.Id, optionId, requestPayload.ToCallbackData());
        }

        private static bool IsJoinAction(BlockAction action) =>
            action.ActionId.StartsWith(BlockBuilder.JoinOptionActionIdPrefix, StringComparison.Ordinal);

        internal static string FallbackUserName(string slackUserId) => $"Slack User {slackUserId}";

        internal static string FallbackChannelName(string slackChannelId) => $"Slack Channel {slackChannelId}";

        public async Task InstallAsync(OAuthTokenResponse response)
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
            var entries = new List<HashEntry>
            {
                new(Constants.FieldKeys.AccessToken, response.AccessToken),
                new(Constants.FieldKeys.InstallTime, timeProvider.GetUtcNow().ToString("O"))
            };

            if (!string.IsNullOrEmpty(response.Team.Name))
                entries.Add(new(Constants.FieldKeys.TeamName, response.Team.Name));

            await hash.SetAsync(hashKey, entries);
        }

        private async Task JoinRunAsync(long userId, long roomId, string optionName, CallbackData callbackData)
        {
            var run = await mediator.Send(new GetCurrentRunQuery(roomId, userId));
            if (run == null)
                throw new SlackTeaTimeException(ErrorStrings.JoinRun_RunNotStarted());

            var group = await mediator.Send(new GetRoomItemGroupQuery(
                RoomId: run.RoomId,
                UserId: userId,
                GroupId: run.GroupId));

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

            await JoinRunInternalAsync(userId, run, option.Id, callbackData);
        }

        private async Task JoinRunAsync(long userId, long roomId, long optionId, CallbackData callbackData)
        {
            var run = await mediator.Send(new GetCurrentRunQuery(roomId, userId));
            if (run == null)
                throw new SlackTeaTimeException(ErrorStrings.JoinRun_RunNotStarted());

            // Buttons on an old round's message can still be clicked, and may be for a different group to the active round
            var option = await mediator.Send(new GetOptionQuery(optionId));
            if (option == null || option.GroupId != run.GroupId)
                throw new SlackTeaTimeException(ErrorStrings.OptionUnknown());

            await JoinRunInternalAsync(userId, run, optionId, callbackData);
        }

        private async Task JoinRunInternalAsync(long userId, Run run, long optionId, CallbackData callbackData)
        {
            BaseCommand command;

            if (run.Ended)
                throw new SlackTeaTimeException(ErrorStrings.JoinRun_RunEnded());

            //check if we need to join or update
            var existingOrder = await mediator.Send(new GetUserOrderQuery(run.Id, userId));
            if (existingOrder == null)
            {
                command = new CreateOrderCommand
                (
                    Id: await idGenerator.GenerateAsync(),
                    RunId: run.Id,
                    UserId: userId,
                    OptionId: optionId
                );
            }
            else
            {
                //we need to update the existing order
                command = new UpdateOrderOptionCommand(existingOrder.Id, userId, optionId);
            }

            command.AddCallbackState(callbackData);

            await mediator.Send(command);
        }
    }
}
