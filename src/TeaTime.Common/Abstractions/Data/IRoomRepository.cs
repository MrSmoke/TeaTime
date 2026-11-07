namespace TeaTime.Common.Abstractions.Data;

using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IRoomRepository
{
    Task CreateAsync(RoomEntity room, CancellationToken cancellationToken = default);
    Task<RoomEntity?> GetAsync(long id, CancellationToken cancellationToken = default);
}
