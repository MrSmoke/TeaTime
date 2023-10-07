namespace TeaTime.Common.Features.Rooms.Queries
{
    using Abstractions;
    using Models.Data;

    public class GetCurrentRunQuery : IUserQuery<Run?>
    {
        public long RoomId { get; }
        public long UserId { get; }

        public GetCurrentRunQuery(long roomId, long userId)
        {
            RoomId = roomId;
            UserId = userId;
        }
    }
}
