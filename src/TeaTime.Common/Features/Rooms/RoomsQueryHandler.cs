namespace TeaTime.Common.Features.Rooms
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using MediatR;
    using Models.Data;
    using Queries;

    public class RoomsQueryHandler(IRoomRepository roomRepository) :
        IRequestHandler<GetRoomQuery, Room?>,
        IRequestHandler<GetRoomByRoomCodeQuery, Room?>
    {
        public Task<Room?> Handle(GetRoomQuery request, CancellationToken cancellationToken)
        {
            return roomRepository.GetAsync(request.RoomId);
        }

        public Task<Room?> Handle(GetRoomByRoomCodeQuery request, CancellationToken cancellationToken)
        {
            return roomRepository.GetByRoomCodeAsync(request.RoomCode);
        }
    }
}
