namespace TeaTime.Data.MySql.Repositories
{
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class RunRepository : BaseRepository, IRunRepository
    {
        public RunRepository(ConnectionFactory factory) : base(factory)
        {
        }

        public Task CreateAsync(Run run)
        {
            const string sql = "INSERT INTO runs " +
                               "(id, roomId, userId, groupId, startTime, endTime, createdDate) VALUES " +
                               "(@id, @roomId, @userId, @groupId, @startTime, @endTime, @createdDate)";

            return ExecuteAsync(sql, run);
        }

        public Task<Run> GetAsync(long runId)
        {
            const string sql =
                "SELECT id, roomId, userId, groupId, startTime, endTime, createdDate FROM runs WHERE id = @runId";

            return SingleOrDefaultAsync<Run>(sql, new {runId});
        }

        public Task CreateResultAsync(RunResult result)
        {
            const string sql = "INSERT INTO run_results " +
                               "(runId, runnerUserId, endedTime) VALUES " +
                               "(@runId, @runnerUserId, @endedTime)";

            return ExecuteAsync(sql, result);
        }
    }
}
