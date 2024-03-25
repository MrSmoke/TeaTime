namespace TeaTime.Data.MySql.Repositories;

using System.Threading.Tasks;
using Common.Abstractions.Data;
using Factories;

public class StatisticsRepository(IMySqlConnectionFactory factory) : BaseRepository(factory), IStatisticsRepository
{
    public Task<long> CountGlobalOrdersMadeAsync()
    {
        const string sql = "SELECT count(*) FROM orders o JOIN runs r ON r.id = o.runId WHERE r.ended = 1";

        return SingleOrDefaultAsync<long>(sql);
    }

    public Task<long> CountGlobalEndedRunsAsync()
    {
        const string sql = "SELECT count(*) FROM runs WHERE ended = 1";

        return SingleOrDefaultAsync<long>(sql);
    }
}