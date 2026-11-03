namespace TeaTime.Common.Abstractions.Data;

using System.Threading;
using System.Threading.Tasks;

public interface ILockRepository
{
    Task<bool> CreateLockAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> DeleteLockAsync(string key, CancellationToken cancellationToken = default);
}
