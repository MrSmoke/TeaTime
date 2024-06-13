namespace TeaTime.Common.Abstractions;

using System.Threading;
using System.Threading.Tasks;

public interface IRoomRunLockService
{
    Task<bool> CreateLockAsync(long roomId, CancellationToken cancellationToken = default);
    Task<bool> DeleteLockAsync(long roomId, CancellationToken cancellationToken = default);
}
