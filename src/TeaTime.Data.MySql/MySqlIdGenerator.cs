namespace TeaTime.Data.MySql;

using System.Threading.Tasks;
using Common.Abstractions;
using Factories;
using Repositories;

public class MySqlIdGenerator(IMySqlConnectionFactory factory) : BaseRepository(factory), IIdGenerator<long>
{
    public async ValueTask<long> GenerateAsync()
    {
        const string sql = "REPLACE INTO ids64 (stub) VALUES ('a'); SELECT LAST_INSERT_ID();";

        return await SingleOrDefaultAsync<long>(sql);
    }
}
