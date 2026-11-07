namespace TeaTime.Common.Abstractions.Data;

using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IRunRepository
{
    Task CreateAsync(Run run, CancellationToken cancellationToken = default);
    Task<Run?> GetAsync(long runId, CancellationToken cancellationToken = default);
    Task<Run?> GetCurrentRunAsync(long roomId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Run run, CancellationToken cancellationToken = default);

    Task CreateResultAsync(RunResult result, CancellationToken cancellationToken = default);
}
