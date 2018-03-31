namespace TeaTime.Data.MySql.Repositories
{
    using System.Threading.Tasks;
    using Common.Abstractions;

    public class MySqlIdGenerator : BaseRepository, IIdGenerator<long>
    {
        public MySqlIdGenerator(ConnectionFactory factory) : base(factory)
        {
        }

        public Task<long> GenerateAsync()
        {
            const string sql = "REPLACE INTO ids64 (stub) VALUES ('a'); SELECT LAST_INSERT_ID();";

            return SingleOrDefault<long>(sql);
        }
    }
}
