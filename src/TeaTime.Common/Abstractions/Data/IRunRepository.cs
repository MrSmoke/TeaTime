namespace TeaTime.Common.Abstractions.Data;

using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IRunRepository
{
    Task CreateAsync(RunEntity run, CancellationToken cancellationToken = default);
    Task<RunEntity?> GetAsync(long runId, CancellationToken cancellationToken = default);
    Task<RunEntity?> GetCurrentRunAsync(long roomId, CancellationToken cancellationToken = default);
    Task UpdateAsync(RunEntity run, CancellationToken cancellationToken = default);

    Task CreateResultAsync(RunResultEntity result, CancellationToken cancellationToken = default);
}
