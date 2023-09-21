namespace TeaTime.Data.MySql.Repositories
{
    using System.Threading.Tasks;
    using Common.Abstractions.Data;

    public class LockRepository : BaseRepository, ILockRepository
    {
        public LockRepository(IMySqlConnectionFactory factory) : base(factory)
        {
        }

        public async Task<bool> CreateLockAsync(string key)
        {
            const string sql = "INSERT IGNORE INTO locks VALUES (@key)";

            var rows = await ExecuteAsync(sql, new {key});

            return rows == 1;
        }

        public async Task<bool> DeleteLockAsync(string key)
        {
            const string sql = "DELETE FROM locks WHERE lockKey = @key";

            var rows = await ExecuteAsync(sql, new { key });

            return rows == 1;
        }
    }
}
