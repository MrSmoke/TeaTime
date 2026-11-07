namespace TeaTime.Common.Features.Rooms;

using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using MediatR;
using Models.Data;
using Queries;

public class RoomsQueryHandler(IRoomRepository roomRepository) : IRequestHandler<GetRoomQuery, RoomEntity?>
{
    public Task<RoomEntity?> Handle(GetRoomQuery request, CancellationToken cancellationToken)
    {
        return roomRepository.GetAsync(request.RoomId, cancellationToken);
    }
}
