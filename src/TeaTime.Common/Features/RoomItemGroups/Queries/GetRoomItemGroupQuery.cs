namespace TeaTime.Common.Features.RoomItemGroups.Queries
{
    using Abstractions;
    using Models;

    public class GetRoomItemGroupQuery : IUserQuery<RoomItemGroupModel>
    {
        public long RoomId { get; }
        public long GroupId { get; }
        public long UserId { get; }

        public GetRoomItemGroupQuery(long roomId, long userId, long groupId)
        {
            RoomId = roomId;
            UserId = userId;
            GroupId = groupId;
        }
    }
}
