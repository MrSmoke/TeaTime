namespace TeaTime.Data.MySql.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class IllMakeRepository : BaseRepository, IIllMakeRepository
    {
        public IllMakeRepository(IMySqlConnectionFactory factory) : base(factory)
        {
        }

        public Task CreateAsync(IllMake obj)
        {
            const string sql =
                "INSERT INTO illmakes (id, runId, userId, createdDate) VALUES (@id, @runId, @userId, @createdDate)";

            return ExecuteAsync(sql, obj);
        }

        public Task<IEnumerable<IllMake>> GetAllByRunAsync(long runId)
        {
            const string sql = "SELECT * FROM illmakes WHERE runId = @runId";

            return QueryAsync<IllMake>(sql, new { runId });
        }
    }
}
