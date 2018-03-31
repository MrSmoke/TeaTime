namespace TeaTime.Slack
{
    using System;
    using System.Threading.Tasks;
    using CommandRouter.Attributes;
    using CommandRouter.Commands;
    using CommandRouter.Results;
    using Common.Features.Users.Commands;
    using Common.Models.Data;
    using MediatR;
    using Models.Requests;
    using Models.Responses;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class SlackCommand : Command
    {
        private readonly IMediator _mediator;

        public SlackCommand(IMediator mediator)
        {
            _mediator = mediator;
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
        public Task<ICommandResult> AddOption(string groupName, string option)
        {
            throw new NotImplementedException();

            //var room = await GetOrCreateRoom();

            //var roomGroup = await _roomService.GetGroupByName(room, groupName);
            //if (roomGroup == null)
            //    return Response($"{groupName} is not a valid teatime group. Please create it first", ResponseType.User);

            //var o = await _roomService.AddOption(roomGroup, option);

            //return Response($"Added option {o.Name} to group {roomGroup.Name}", ResponseType.User);
        }

        [Command("tea")]
        public async Task<ICommandResult> Start(string group = "tea")
        {
            var user = GetOrCreateUser();
            var room = await GetOrCreateRoom();

            var roomGroup = await _roomService.GetGroupByName(room, group);
            if(roomGroup == null)
                return Response($"{group} is not a valid teatime group. Please create it first", ResponseType.User);

            await _runService.Start(room, await user, roomGroup);

            var options = await _roomService.GetOptions(roomGroup);

            var attachments = AttachmentBuilder.BuildOptions(options);

            return Response(new SlashCommandResponse
            {
                Text = $"{(await user).DisplayName} wants tea",
                Type = ResponseType.Channel,
                Attachments = attachments
            });
        }

        [Command("join")]
        public async Task<SlashCommandResponse> Join()
        {
            var user = GetOrCreateUser();
            var room = await GetOrCreateRoom();

            await _runService.Join(room, await user);

            return new SlashCommandResponse("You have joined this round of tea!", ResponseType.User);
        }

        public Task IllMake()
        {
            throw new NotImplementedException();
        }

        [Command("end")]
        public async Task<SlashCommandResponse> End()
        {
            var user = await GetOrCreateUser();
            var room = await GetOrCreateRoom();

            var runEndResult = await _runService.End(room, user);

            throw new NotImplementedException();
        }

        private async Task<User> GetOrCreateUser()
        {
            var user = await _userService.GetByLink(Command.UserId);
            if (user != null)
                return user;

            var command = new CreateUserCommand
            {
                Username = Command.UserName,
                DisplayName = Command.UserName,
            };
            await _mediator.Send(command).ConfigureAwait(false);

            await _userService.AddLink(Command.UserId, user);

            return user;
        }

        private async Task<Room> GetOrCreateRoom()
        {
            var room = await _roomService.GetByLink(Command.ChannelId);
            if (room != null)
                return room;

            room = await _roomService.Create(Command.ChannelName);
            await _roomService.AddLink(Command.ChannelId, room);

            return room;
        }

        public SlashCommand Command => (SlashCommand) Context.Items["SLASHCOMMAND"];

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
