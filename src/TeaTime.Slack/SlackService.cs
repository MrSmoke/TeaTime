namespace TeaTime.Slack
{
    using System;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Services;
    using Models.Requests;
    using Models.Responses;

    public class SlackService
    {
        private readonly IUserService _userService;
        private readonly IRunService _runService;
        private readonly IRoomService _roomService;

        public SlackService(IUserService userService, IRunService runService, IRoomService roomService)
        {
            _userService = userService;
            _runService = runService;
            _roomService = roomService;
        }

        public async Task<SlashCommandResponse> Start()
        {
            //Only support tea for now
            const string group = "tea";

            var user = GetOrCreateUser();
            var room = await GetOrCreateRoom();

            var roomGroup = await _roomService.GetGroupByName(room, group);

            if(roomGroup == null)
                return new SlashCommandResponse($"{group} is not a valid teatime group. Please create it first", ResponseType.User);

            await _runService.Start(room, await user, roomGroup);

            var options = await _roomService.GetOptions(roomGroup);

            var attachments = AttachmentBuilder.BuildOptions(options);

            return new SlashCommandResponse
            {
                Text = $"{(await user).Name} wants tea",
                Type = ResponseType.Channel,
                Attachments = attachments
            };
        }

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

            user = await _userService.Create(Command.UserName);
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

        public SlashCommand Command { get; set; }
    }
}
