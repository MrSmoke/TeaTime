namespace TeaTime.Slack
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using CommandRouter.Attributes;
    using CommandRouter.Commands;
    using CommandRouter.Results;
    using Common.Abstractions;
    using Common.Features.Links.Commands;
    using Common.Features.Links.Queries;
    using Common.Features.Options.Commands;
    using Common.Features.RoomItemGroups.Queries;
    using Common.Features.Rooms.Commands;
    using Common.Features.Rooms.Queries;
    using Common.Features.Runs.Commands;
    using Common.Features.Runs.Events;
    using Common.Features.Runs.Queries;
    using Common.Features.Users.Commands;
    using Common.Features.Users.Queries;
    using Common.Models;
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
        private readonly IRunEventListner _runEventListner;

        public SlackCommand(IMediator mediator, IIdGenerator<long> idGenerator, ISystemClock clock, IRunEventListner runEventListner)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
            _clock = clock;
            _runEventListner = runEventListner;
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

            var group = _mediator.Send(new GetRoomItemGroupByNameQuery(
                roomId: room.Id,
                userId: user.Id,
                name: groupName));

            if(group == null)
                return Response($"{groupName} is not a valid teatime group. Please create it first", ResponseType.User);

            var command =
                new CreateOptionCommand(
                    id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                    userId: user.Id,
                    name: optionName);

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

            var command = new StartRunCommand(runId, user.Id, room.Id, 0, _clock.UtcNow());

            await _mediator.Send(command).ConfigureAwait(false);

            return Response(new SlashCommandResponse
            {
                Text = $"{user.DisplayName} wants tea",
                Type = ResponseType.Channel,
                Attachments = AttachmentBuilder.BuildOptions(roomItemGroup.Options)
            });
        }

        [Command("join")]
        public async Task<ICommandResult> Join(string optionText)
        {
            var slashCommand = GetCommand();

            var user = await GetOrCreateUser(slashCommand).ConfigureAwait(false);
            var room = await GetOrCreateRoom(slashCommand, user.Id).ConfigureAwait(false);
            var run = await _mediator.Send(new GetCurrentRunQuery(room.Id, user.Id)).ConfigureAwait(false);

            var group = await _mediator.Send(new GetRoomItemGroupQuery(roomId: run.RoomId, userId: user.Id, groupId: run.GroupId)).ConfigureAwait(false);

            var option = group.Options.FirstOrDefault(o => o.Name.Equals(optionText));
            if (option == null)
                return Response($"Unknown option '{optionText}'", ResponseType.User);

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

            await _mediator.Send(command).ConfigureAwait(false);

            //todo: how to return?
            //not the best but it will do for now
            var response = await _runEventListner.WaitOnceAsync<RunEndedEvent, SlashCommandResponse>(
                async evt =>
                {
                    var runner = await _mediator.Send(new GetUserQuery(evt.RunnerUserId)).ConfigureAwait(false);

                    return new SlashCommandResponse
                    {
                        Text = "Run ended. Lucky person is @" + runner.DisplayName,
                        Type = ResponseType.Channel
                    };

                }, TimeSpan.FromSeconds(5)).ConfigureAwait(false);

            return Response(response);
        }

        private async Task<User> GetOrCreateUser(SlashCommand slashCommand)
        {
            var userId = await _mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.User, slashCommand.UserId)).ConfigureAwait(false);
            if (userId > 0)
                return await _mediator.Send(new GetUserQuery(userId)).ConfigureAwait(false);

            //we need to create a new user
            var command = new CreateUserCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                username: "slack_" + slashCommand.UserId,
                displayName: slashCommand.UserName
            );
            await _mediator.Send(command).ConfigureAwait(false);

            //add link
            await _mediator.Send(new CreateLinkCommand(command.Id, LinkType.User, slashCommand.UserId)).ConfigureAwait(false);

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

        private async Task<Room> GetOrCreateRoom(SlashCommand slashCommand, long userId)
        {
            var roomId = await _mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.Room, slashCommand.ChannelId)).ConfigureAwait(false);
            if (roomId > 0)
                return await _mediator.Send(new GetRoomQuery(roomId)).ConfigureAwait(false);

            var command = new CreateRoomCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                name: slashCommand.ChannelName,
                userId: userId
            );

            await _mediator.Send(command).ConfigureAwait(false);

            //add link
            await _mediator.Send(new CreateLinkCommand(command.Id, LinkType.Room, slashCommand.ChannelId)).ConfigureAwait(false);

            return new Room
            {
                Id = command.Id,
                Name = command.Name,
                CreatedDate = DateTimeOffset.MinValue //dunno this value *shrug*
            };
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
