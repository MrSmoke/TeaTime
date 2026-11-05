namespace TeaTime.Slack.Commands;

using System;
using System.Linq;
using System.Threading.Tasks;
using CommandRouter.Attributes;
using CommandRouter.Results;
using Common.Abstractions;
using Common.Features.Orders.Queries;
using Common.Features.RoomItemGroups.Queries;
using Common.Features.Rooms.Queries;
using Common.Features.Runs.Commands;
using Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Models.SlashCommands;
using Resources;
using Services;

public class RunCommand(
    IMediator mediator,
    IIdGenerator<long> idGenerator,
    TimeProvider clock,
    ISlackService slackService,
    ILogger<RunCommand> logger)
    : BaseCommand(slackService)
{
    [Command("start")]
    public async Task<ICommandResult> Start(string group = "tea")
    {
        var context = await GetContextAsync();

        var roomItemGroup = await mediator.Send(new GetRoomItemGroupByNameQuery(
            RoomId: context.Room.Id,
            UserId: context.User.Id,
            Name: group));

        if (roomItemGroup == null)
            return Response(ErrorStrings.StartRun_GroupInvalidName(group), ResponseType.User);

        if (!roomItemGroup.Options.Any())
            return Response(ErrorStrings.StartRun_GroupNoOptions(roomItemGroup.Name), ResponseType.User);

        var command = new StartRunCommand
        (
            Id: await idGenerator.GenerateAsync(),
            UserId: context.User.Id,
            RoomId: context.Room.Id,
            RoomGroupId: roomItemGroup.Id,
            StartTime: clock.GetUtcNow()
        );

        await mediator.Send(command);

        var text = ResponseStrings.RunStarted(context.Command.UserId, roomItemGroup.Name);

        return Response(new SlashCommandResponse
        {
            Text = text,
            Type = ResponseType.Channel,
            Blocks = BlockBuilder.BuildRunStarted(text, roomItemGroup.Options)
        });
    }

    [Command("join")]
    public async Task<ICommandResult> Join(string optionName)
    {
        var slashCommand = GetCommand();

        try
        {
            await SlackService.JoinRunAsync(slashCommand, optionName);

            return Ok();
        }
        catch (SlackTeaTimeException e)
        {
            logger.LogInformation("Join run rejected: {Message}", e.Message);
            return Response(e.Message, ResponseType.User);
        }
    }

    [Command("leave")]
    public async Task<ICommandResult> Leave()
    {
        var slashCommand = GetCommand();

        try
        {
            await SlackService.LeaveRunAsync(slashCommand);

            return Ok();
        }
        catch (SlackTeaTimeException e)
        {
            logger.LogInformation(e, "Leave run rejected");
            return Response(e.Message, ResponseType.User);
        }
    }

    [Command("end")]
    public async Task<ICommandResult> End()
    {
        var context = await GetContextAsync();

        var run = await mediator.Send(new GetCurrentRunQuery(context.Room.Id, context.User.Id));
        if (run == null)
            return Response(ErrorStrings.EndRun_RunNotStarted(), ResponseType.User);

        var orders = await mediator.Send(new GetRunOrdersQuery(run.Id, context.User.Id));

        var command = new EndRunCommand(
            Run: run,
            UserId: context.User.Id,
            Orders: orders
        );

        command.AddCallbackState(context.Command.ToCallbackData());

        await mediator.Send(command);

        return Response(null, ResponseType.Channel);
    }
}
