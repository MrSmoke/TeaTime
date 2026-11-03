namespace TeaTime.Common.Abstractions.Data;

using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IRoomRepository
{
    Task CreateAsync(Room room, CancellationToken cancellationToken = default);
    Task<Room?> GetAsync(long id, CancellationToken cancellationToken = default);
}
