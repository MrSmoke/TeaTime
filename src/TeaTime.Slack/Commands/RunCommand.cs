namespace TeaTime.Slack.Commands
{
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
    using Models.Responses;
    using Resources;
    using Services;

    public class RunCommand : BaseCommand
    {
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;
        private readonly ISystemClock _clock;
        private readonly ISlackService _slackService;

        public RunCommand(IMediator mediator,
            IIdGenerator<long> idGenerator,
            ISystemClock clock,
            ISlackService slackService) : base(slackService)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
            _clock = clock;
            _slackService = slackService;
        }

        [Command("")]
        public async Task<ICommandResult> Start(string group = "tea")
        {
            var context = await GetContextAsync();

            var roomItemGroup = await _mediator.Send(new GetRoomItemGroupByNameQuery(
                RoomId: context.Room.Id,
                UserId: context.User.Id,
                Name: group));

            if (roomItemGroup == null)
                return Response(ErrorStrings.StartRun_GroupInvalidName(group), ResponseType.User);

            if(!roomItemGroup.Options.Any())
                return Response(ErrorStrings.StartRun_GroupNoOptions(roomItemGroup.Name), ResponseType.User);

            var command = new StartRunCommand(
                Id: await _idGenerator.GenerateAsync(),
                UserId: context.User.Id,
                RoomId: context.Room.Id,
                RoomGroupId: roomItemGroup.Id,
                StartTime: _clock.UtcNow());

            await _mediator.Send(command);

            return Response(new SlashCommandResponse
            {
                Text = ResponseStrings.RunStarted(context.Command.UserId, roomItemGroup.Name),
                Type = ResponseType.Channel,
                Attachments = AttachmentBuilder.BuildOptions(roomItemGroup.Options)
            });
        }

        [Command("join")]
        public async Task<ICommandResult> Join(string optionName)
        {
            var slashCommand = GetCommand();

            try
            {
                await _slackService.JoinRunAsync(slashCommand, optionName);

                return Ok();
            }
            catch (SlackTeaTimeException e)
            {
                return Response(e.Message, ResponseType.User);
            }
        }

        [Command("end")]
        public async Task<ICommandResult> End()
        {
            var context = await GetContextAsync();

            var run = await _mediator.Send(new GetCurrentRunQuery(context.Room.Id, context.User.Id));
            if (run == null)
                return Response(ErrorStrings.EndRun_RunNotStarted(), ResponseType.User);

            var orders = await _mediator.Send(new GetRunOrdersQuery(run.Id, context.User.Id));

            var command = new EndRunCommand(
                Run: run,
                UserId: context.User.Id,
                Orders: orders
            );

            command.AddCallbackState(context.Command.ToCallbackData());

            await _mediator.Send(command);

            return Response(null, ResponseType.Channel);
        }
    }
}
