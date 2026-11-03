namespace TeaTime.Data.MySql.Repositories;

using System.Threading;
using System.Threading.Tasks;
using Common.Abstractions.Data;
using Factories;

public class StatisticsRepository(IMySqlConnectionFactory factory) : BaseRepository(factory), IStatisticsRepository
{
    public Task<long> CountGlobalOrdersMadeAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT count(*) FROM orders o
                           JOIN runs r ON r.id = o.runId
                           WHERE r.ended = 1 AND o.deleted = 0
                           """;

        return SingleOrDefaultAsync<long>(sql, cancellationToken: cancellationToken);
    }

    public Task<long> CountGlobalEndedRunsAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT count(*) FROM runs WHERE ended = 1";

        return SingleOrDefaultAsync<long>(sql, cancellationToken: cancellationToken);
    }
}
