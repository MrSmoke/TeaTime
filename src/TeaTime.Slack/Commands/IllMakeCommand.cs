namespace TeaTime.Slack.Commands;

using System.Threading.Tasks;
using CommandRouter.Attributes;
using CommandRouter.Results;
using Common.Abstractions;
using Common.Features.Rooms.Queries;
using MediatR;
using Models.SlashCommands;
using Resources;
using Services;

public class IllMakeCommand(
    ISlackService slackService,
    IMediator mediator,
    IIdGenerator<long> idGenerator)
    : BaseCommand(slackService)
{
    [Command("illmake")]
    public async Task<ICommandResult> IllMake()
    {
        var context = await GetContextAsync();

        var run = await mediator.Send(new GetCurrentRunQuery(context.Room.Id, context.User.Id));
        if (run == null)
            return Response(ErrorStrings.IllMake_RunNotStarted(), ResponseType.User);

        var command = new Common.Features.IllMake.Commands.IllMakeCommand(
            Id: await idGenerator.GenerateAsync(),
            RunId: run.Id,
            CurrentUserId: context.User.Id
        );

        await mediator.Send(command);

        return Response(new SlashCommandResponse
        {
            Text = ResponseStrings.IllMake(context.Command.UserId),
            Type = ResponseType.Channel
        });
    }
}
