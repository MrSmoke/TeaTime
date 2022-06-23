namespace TeaTime.Common.Features.RoomItemGroups.Queries
{
    using Abstractions;
    using Models;

    public class GetRoomItemGroupByNameQuery : IUserQuery<RoomItemGroupModel?>
    {
        public long RoomId { get; }
        public string Name { get; }
        public long UserId { get; }

        public GetRoomItemGroupByNameQuery(long roomId, long userId, string name)
        {
            RoomId = roomId;
            UserId = userId;
            Name = name;
        }
    }
}
