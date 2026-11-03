namespace TeaTime.Common.Abstractions.Data;

using System.Threading;
using System.Threading.Tasks;

public interface IStatisticsRepository
{
    /// <summary>
    /// Gets the count of all orders made from ended runs
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> CountGlobalOrdersMadeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of all runs which have ended
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> CountGlobalEndedRunsAsync(CancellationToken cancellationToken = default);
}
