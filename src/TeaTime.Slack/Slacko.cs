namespace TeaTime.Slack
{
    using System;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Services;

    public class Slacko
    {
        private readonly IUserService _userService;
        private readonly IRunService _runService;
        private readonly IRoomService _roomService;

        public Slacko(IUserService userService, IRunService runService, IRoomService roomService)
        {
            _userService = userService;
            _runService = runService;
            _roomService = roomService;
        }

        public async Task Start(string group = "tea")
        {


            var user = await GetOrCreateUser();
            var room = await GetOrCreateRoom();

            var run = _runService.Start(room, user, group);

            var options = await _roomService.GetOptions(room, group);

            var attachments = AttachmentBuilder.BuildOptions(options);

            throw new NotImplementedException();
        }

        public Task Join()
        {
            throw new NotImplementedException();
        }

        public Task IllMake()
        {
            throw new NotImplementedException();
        }

        public Task End()
        {
            throw new NotImplementedException();
        }

        private async Task<User> GetOrCreateUser()
        {
            var user = await _userService.GetByLink(Command.UserId);
            if (user != null)
                return user;

            user = await _userService.Create(new User());
            await _userService.AddLink(Command.UserId, user);

            return user;
        }

        private async Task<Room> GetOrCreateRoom()
        {
            var room = await _roomService.GetByLink(Command.ChannelId);
            if (room != null)
                return room;

            room = await _roomService.Create();
            await _roomService.AddLink(Command.ChannelId, room);

            return room;
        }

        private SlashCommand Command => new SlashCommand();
    }
}
