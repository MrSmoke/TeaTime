namespace TeaTime.Data.MySql;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Collections;
using Factories;
using Repositories;

public class MySqlDistributedHash(IMySqlConnectionFactory factory) : BaseRepository(factory), IDistributedHash
{
    public Task SetAsync(string key, string field, string value, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO hashes (`Key`, `Field`, `Value`) VALUES (@key, @field, @value) " +
                           "ON DUPLICATE KEY UPDATE `Value`=@value";

        return ExecuteAsync(sql, new
        {
            key,
            field,
            value
        }, cancellationToken);
    }

    public Task SetAsync(string key, IEnumerable<HashEntry> fields, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO hashes (`Key`, `Field`, `Value`) VALUES (@key, @field, @value) " +
                           "ON DUPLICATE KEY UPDATE `Value`=@value";

        return ExecuteAsync(sql, fields.Select(f => new
        {
            key,
            f.Field,
            f.Value
        }), cancellationToken);
    }

    public Task<IEnumerable<HashEntry>> GetAllAsync(string key, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Field, Value FROM hashes WHERE `Key` = @key";

        return QueryAsync<HashEntry>(sql, new
        {
            key
        }, cancellationToken);
    }

    public Task<string?> GetValueAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Value FROM hashes WHERE `Key` = @key AND Field = @field";

        return SingleOrDefaultAsync<string>(sql, new
        {
            key,
            field
        }, cancellationToken);
    }
}
