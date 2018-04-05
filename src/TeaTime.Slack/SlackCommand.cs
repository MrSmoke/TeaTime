namespace TeaTime.Slack
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using CommandRouter.Attributes;
    using CommandRouter.Commands;
    using CommandRouter.Results;
    using Common.Abstractions;
    using Common.Features.Options.Commands;
    using Common.Features.RoomItemGroups.Queries;
    using Common.Features.Rooms.Queries;
    using Common.Features.Runs.Commands;
    using Common.Features.Runs.Events;
    using Common.Features.Runs.Queries;
    using Common.Features.Users.Queries;
    using Common.Models.Data;
    using MediatR;
    using Models.Requests;
    using Models.Responses;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class SlackCommand : Command
    {
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;
        private readonly ISystemClock _clock;
        private readonly IRunEventListener _runEventListener;
        private readonly ISlackService _slackService;

        public SlackCommand(IMediator mediator, IIdGenerator<long> idGenerator, ISystemClock clock, IRunEventListener runEventListener, ISlackService slackService)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
            _clock = clock;
            _runEventListener = runEventListener;
            _slackService = slackService;
        }

        [Command("addgroup")]
        public Task<ICommandResult> AddGroup(string name)
        {
            throw new NotImplementedException();

            //var room = await GetOrCreateRoom();
            //var group = await _roomService.AddGroup(room, name);

            //return Response($"Group `{group.Name}` created", ResponseType.User);
        }

        [Command("addoption")]
        public async Task<ICommandResult> AddOption(string groupName, string optionName)
        {
            var slashCommand = GetCommand();

            var user = await GetOrCreateUser(slashCommand).ConfigureAwait(false);
            var room = await GetOrCreateRoom(slashCommand, user.Id).ConfigureAwait(false);

            var group = await _mediator.Send(new GetRoomItemGroupByNameQuery(
                roomId: room.Id,
                userId: user.Id,
                name: groupName)).ConfigureAwait(false);

            if(group == null)
                return Response($"{groupName} is not a valid teatime group. Please create it first", ResponseType.User);

            var command =
                new CreateOptionCommand(
                    id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                    userId: user.Id,
                    groupId: group.Id,
                    name: optionName);

            await _mediator.Send(command).ConfigureAwait(false);

            return Response($"Added option {optionName} to group {room.Name}", ResponseType.User);
        }

        [Command("")]
        public async Task<ICommandResult> Start(string group = "tea")
        {
            var slashCommand = GetCommand();

            var user = await GetOrCreateUser(slashCommand).ConfigureAwait(false);
            var room = await GetOrCreateRoom(slashCommand, user.Id).ConfigureAwait(false);

            var roomItemGroup = await _mediator.Send(new GetRoomItemGroupByNameQuery(roomId: room.Id, userId: user.Id, name: group)).ConfigureAwait(false);
            if (roomItemGroup == null)
                return Response($"{group} is not a valid teatime group. Please create it first", ResponseType.User);

            if(!roomItemGroup.Options.Any())
                return Response($"You must add options to the group {group} before using it", ResponseType.User);

            var runId = await _idGenerator.GenerateAsync().ConfigureAwait(false);

            var command = new StartRunCommand(runId, user.Id, room.Id, roomItemGroup.Id, _clock.UtcNow());

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(new SlashCommandResponse
            {
                Text = $"{user.DisplayName} wants tea",
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
                return Response($"Please start first", ResponseType.User);

            var group = await _mediator.Send(new GetRoomItemGroupQuery(roomId: run.RoomId, userId: user.Id, groupId: run.GroupId)).ConfigureAwait(false);

            var option = group.Options.FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));
            if (option == null)
                return Response($"Unknown option '{optionName}'", ResponseType.User);

            var command = new JoinRunCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                runId: run.Id,
                userId: user.Id,
                optionId: option.Id);

            await _mediator.Send(command).ConfigureAwait(false);

            return Response($"{user.DisplayName} has joined this round", ResponseType.Channel);
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
            );

            //todo: how to return?
            //not the best but it will do for now
            var responseTask = _runEventListener.WaitOnceAsync<RunEndedEvent>(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

            await _mediator.Send(command).ConfigureAwait(false);

            var endedEvent = await responseTask;

            var runner = await _mediator.Send(new GetUserQuery(endedEvent.RunnerUserId)).ConfigureAwait(false);

            return Response("Run ended. Lucky person is @" + runner.DisplayName, ResponseType.Channel);
        }

        private Task<User> GetOrCreateUser(SlashCommand slashCommand)
        {
            return _slackService.GetOrCreateUser(slashCommand.UserId, slashCommand.UserName);
        }

        private Task<Room> GetOrCreateRoom(SlashCommand slashCommand, long userId)
        {
            return _slackService.GetOrCreateRoom(slashCommand.ChannelId, slashCommand.ChannelName, userId);
        }

        public SlashCommand GetCommand() => (SlashCommand) Context.Items["SLASHCOMMAND"];

        protected ICommandResult Response(string text, ResponseType responseType)
        {
            return Response(new SlashCommandResponse(text, responseType));
        }

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        protected ICommandResult Response(SlashCommandResponse response)
        {
            return StringResult(JsonConvert.SerializeObject(response, JsonSettings));
        }
    }
}
