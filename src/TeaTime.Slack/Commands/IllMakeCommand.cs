namespace TeaTime.Slack.Commands
{
    using System.Threading.Tasks;
    using CommandRouter.Attributes;
    using CommandRouter.Results;
    using Common.Abstractions;
    using Common.Features.Rooms.Queries;
    using MediatR;
    using Models.Responses;
    using Resources;
    using Services;

    public class IllMakeCommand : BaseCommand
    {
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;

        public IllMakeCommand(ISlackService slackService, IMediator mediator, IIdGenerator<long> idGenerator) : base(slackService)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
        }

        [Command("illmake")]
        public async Task<ICommandResult> IllMake()
        {
            var context = await GetContextAsync().ConfigureAwait(false);

            var run = await _mediator.Send(new GetCurrentRunQuery(context.Room.Id, context.User.Id)).ConfigureAwait(false);
            if(run == null)
                return Response(ErrorStrings.IllMake_RunNotStarted(), ResponseType.User);

            var command = new Common.Features.IllMake.Commands.IllMakeCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                runId: run.Id,
                userId: context.User.Id
            );

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(new SlashCommandResponse
            {
                Text = ResponseStrings.IllMake(context.Command.UserId),
                Type = ResponseType.Channel
            });
        }
    }
}
