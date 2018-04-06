namespace TeaTime.Slack.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using CommandRouter.Attributes;
    using CommandRouter.Results;
    using Common.Abstractions;
    using Common.Features.RoomItemGroups.Queries;
    using Common.Features.Rooms.Queries;
    using Common.Features.Runs.Commands;
    using Common.Features.Runs.Queries;
    using MediatR;
    using Models.Responses;
    using Resources;
    using Services;

    public class RunCommand : BaseCommand
    {
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;
        private readonly ISystemClock _clock;

        public RunCommand(IMediator mediator, IIdGenerator<long> idGenerator, ISystemClock clock, ISlackService slackService) : base(slackService)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
            _clock = clock;
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

            var user = await GetOrCreateUser(slashCommand).ConfigureAwait(false);
            var room = await GetOrCreateRoom(slashCommand, user.Id).ConfigureAwait(false);

            var run = await _mediator.Send(new GetCurrentRunQuery(room.Id, user.Id)).ConfigureAwait(false);
            if (run == null)
                return Response(ErrorStrings.JoinRun_RunNotStarted(), ResponseType.User);

            var group = await _mediator.Send(new GetRoomItemGroupQuery(roomId: run.RoomId, userId: user.Id, groupId: run.GroupId)).ConfigureAwait(false);
            if (group == null)
            {
                //todo: log error
                return Response(ErrorStrings.General(), ResponseType.User);
            }

            var option = group.Options.FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));
            if (option == null)
                return Response(ErrorStrings.JoinRun_OptionUnknown(optionName, group.Name), ResponseType.User);

            var command = new JoinRunCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                runId: run.Id,
                userId: user.Id,
                optionId: option.Id);

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(ResponseStrings.RunUserJoined(slashCommand.UserId), ResponseType.Channel);
        }

        public Task IllMake()
        {
            throw new NotImplementedException();
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
            )
            {
                State = GetState()
            };

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(null, ResponseType.Channel);
        }
    }
}
