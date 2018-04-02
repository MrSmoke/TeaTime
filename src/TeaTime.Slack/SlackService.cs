namespace TeaTime.Slack
{
    using System;
    using System.Threading.Tasks;
    using CommandRouter.Attributes;
    using CommandRouter.Commands;
    using CommandRouter.Results;
    using Common.Abstractions;
    using Common.Features.Links.Commands;
    using Common.Features.Links.Queries;
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

        public SlackCommand(IMediator mediator, IIdGenerator<long> idGenerator)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
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

        [Command("start")]
        public Task<ICommandResult> Start(string group = "tea")
        {
            throw new NotImplementedException();

            //var user = GetOrCreateUser();
            //var room = await GetOrCreateRoom();

            //var roomGroup = await _roomService.GetGroupByName(room, group);
            //if(roomGroup == null)
            //    return Response($"{group} is not a valid teatime group. Please create it first", ResponseType.User);

            //await _runService.Start(room, await user, roomGroup);

            //var options = await _roomService.GetOptions(roomGroup);

            //var attachments = AttachmentBuilder.BuildOptions(options);

            //return Response(new SlashCommandResponse
            //{
            //    Text = $"{(await user).DisplayName} wants tea",
            //    Type = ResponseType.Channel,
            //    Attachments = attachments
            //});
        }

        [Command("join")]
        public Task<SlashCommandResponse> Join()
        {
            throw new NotImplementedException();

            //var user = GetOrCreateUser();
            //var room = await GetOrCreateRoom();

            //await _runService.Join(room, await user);

            //return new SlashCommandResponse("You have joined this round of tea!", ResponseType.User);
        }

        public Task IllMake()
        {
            throw new NotImplementedException();
        }

        [Command("end")]
        public async Task<SlashCommandResponse> End()
        {
            throw new NotImplementedException();
        }

        private async Task<User> GetOrCreateUser()
        {
            var userId = await _mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.User, Command.UserId)).ConfigureAwait(false);
            if (userId > 0)
                return await _mediator.Send(new GetUserQuery(userId)).ConfigureAwait(false);

            //we need to create a new user
            var command = new CreateUserCommand
            {
                Id = await _idGenerator.GenerateAsync().ConfigureAwait(false),
                Username = "slack_" + Command.UserId,
                DisplayName = Command.UserName
            };
            await _mediator.Send(command).ConfigureAwait(false);

            //add link
            await _mediator.Send(new CreateLinkCommand(command.Id, LinkType.User, Command.UserId)).ConfigureAwait(false);

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

        private async Task<Room> GetOrCreateRoom()
        {
            throw new NotImplementedException();
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
