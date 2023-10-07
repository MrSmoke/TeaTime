namespace TeaTime.Common.Features.Rooms.Queries
{
    using Abstractions;
    using Models.Data;

    public class GetRoomQuery : IQuery<Room?>
    {
        public long RoomId { get; }

        public GetRoomQuery(long roomId)
        {
            RoomId = roomId;
        }
    }
}
