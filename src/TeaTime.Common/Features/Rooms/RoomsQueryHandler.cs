namespace TeaTime.Common.Features.Rooms
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using MediatR;
    using Models.Data;
    using Queries;

    public class RoomsQueryHandler : IRequestHandler<GetRoomQuery, Room?>
    {
        private readonly IRoomRepository _roomRepository;

        public RoomsQueryHandler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public Task<Room?> Handle(GetRoomQuery request, CancellationToken cancellationToken)
        {
            return _roomRepository.GetAsync(request.RoomId);
        }
    }
}
