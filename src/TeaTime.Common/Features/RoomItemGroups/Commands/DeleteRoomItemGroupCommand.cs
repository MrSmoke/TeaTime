namespace TeaTime.Common.Features.RoomItemGroups.Commands
{
    using Abstractions;

    public class DeleteRoomItemGroupCommand : BaseCommand, IUserCommand
    {
        public long GroupId { get; }
        public long UserId { get; }

        public DeleteRoomItemGroupCommand(long groupId, long userId)
        {
            GroupId = groupId;
            UserId = userId;
        }
    }
}
