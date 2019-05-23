namespace TeaTime.Common.Features.RoomItemGroups.Commands
{
    using Abstractions;

    public class CreateRoomItemGroupCommand : IUserCommand
    {
        public long Id { get; }
        public long RoomId { get; }
        public string Name { get; }
        public long UserId { get; }

        public CreateRoomItemGroupCommand(long id, long roomId, string name, long userId)
        {
            Id = id;
            RoomId = roomId;
            Name = name;
            UserId = userId;
        }
    }
}
