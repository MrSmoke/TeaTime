namespace TeaTime.Data.MySql.Repositories;

using System.Threading.Tasks;
using Common.Abstractions.Data;
using Common.Models.Data;
using Factories;

public class RunRepository(IMySqlConnectionFactory factory) : BaseRepository(factory), IRunRepository
{
    private const string Columns = "id, roomId, userId, groupId, startTime, endTime, ended, createdDate";

    public Task CreateAsync(Run run)
    {
        const string sql = "INSERT INTO runs " +
                           "(" + Columns + ") VALUES " +
                           "(@id, @roomId, @userId, @groupId, @startTime, @endTime, @ended, @createdDate)";

        return ExecuteAsync(sql, run);
    }

    public Task<Run?> GetAsync(long runId)
    {
        const string sql =
            "SELECT " + Columns + " FROM runs WHERE id = @runId";

        return SingleOrDefaultAsync<Run>(sql, new { runId });
    }

    public Task<Run?> GetCurrentRunAsync(long roomId)
    {
        const string sql =
            "SELECT " + Columns + " FROM runs WHERE roomId = @roomId AND ended = 0 order by createdDate desc";

        return QueryFirstOrDefaultAsync<Run>(sql, new { roomId });
    }

    public Task UpdateAsync(Run run)
    {
        const string sql = "UPDATE runs SET ended = @ended WHERE id = @id";

        return ExecuteAsync(sql, run);
    }

    public Task CreateResultAsync(RunResult result)
    {
        const string sql = "INSERT INTO run_results " +
                           "(runId, runnerUserId, endedTime) VALUES " +
                           "(@runId, @runnerUserId, @endedTime)";

        return ExecuteAsync(sql, result);
    }
}
