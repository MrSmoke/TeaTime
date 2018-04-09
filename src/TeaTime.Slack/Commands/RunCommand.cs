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
    using Microsoft.Extensions.Logging;
    using Models.Responses;
    using Resources;
    using Services;

    public class RunCommand : BaseCommand
    {
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;
        private readonly ISystemClock _clock;
        private readonly ISlackService _slackService;
        private readonly ILogger<RunCommand> _logger;

        public RunCommand(IMediator mediator, IIdGenerator<long> idGenerator, ISystemClock clock, ISlackService slackService, ILogger<RunCommand> logger) : base(slackService)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
            _clock = clock;
            _slackService = slackService;
            _logger = logger;
        }

        [Command("")]
        public async Task<ICommandResult> Start(string group = "tea")
        {
            var slashCommand = GetCommand();

            var user = await GetOrCreateUser(slashCommand).ConfigureAwait(false);
            var room = await GetOrCreateRoom(slashCommand, user.Id).ConfigureAwait(false);

            var roomItemGroup = await _mediator.Send(new GetRoomItemGroupByNameQuery(roomId: room.Id, userId: user.Id, name: group)).ConfigureAwait(false);
            if (roomItemGroup == null)
                return Response(ErrorStrings.StartRun_GroupInvalidName(group), ResponseType.User);

            if(!roomItemGroup.Options.Any())
                return Response(ErrorStrings.StartRun_GroupNoOptions(roomItemGroup.Name), ResponseType.User);

            var command = new StartRunCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                userId: user.Id,
                roomId: room.Id,
                roomGroupId: roomItemGroup.Id,
                startTime: _clock.UtcNow());

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(new SlashCommandResponse
            {
                Text = ResponseStrings.RunStarted(slashCommand.UserId, roomItemGroup.Name),
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
                await _slackService.JoinRunAsync(slashCommand, optionName).ConfigureAwait(false);

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
            var slashCommand = GetCommand();

            var user = await GetOrCreateUser(slashCommand).ConfigureAwait(false);
            var room = await GetOrCreateRoom(slashCommand, user.Id).ConfigureAwait(false);

            var run = await _mediator.Send(new GetCurrentRunQuery(room.Id, user.Id)).ConfigureAwait(false);
            var orders = await _mediator.Send(new GetRunOrdersQuery(run.Id, user.Id)).ConfigureAwait(false);

            var command = new EndRunCommand(
                runId: run.Id,
                roomId: room.Id,
                userId: user.Id,
                orders: orders
            );

            command.AddCallbackState(slashCommand.ToCallbackData());

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(null, ResponseType.Channel);
        }
    }
}
