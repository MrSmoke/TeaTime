namespace TeaTime.Data.MySql;

using System.Threading.Tasks;
using Common.Abstractions;
using Factories;
using Repositories;

public class MySqlIdGenerator(IMySqlConnectionFactory factory) : BaseRepository(factory), IIdGenerator<long>
{
    public Task<long> GenerateAsync()
    {
        const string sql = "REPLACE INTO ids64 (stub) VALUES ('a'); SELECT LAST_INSERT_ID();";

        return SingleOrDefaultAsync<long>(sql);
    }
}
