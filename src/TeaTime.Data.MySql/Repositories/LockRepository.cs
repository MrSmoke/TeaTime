namespace TeaTime.Data.MySql.Repositories;

using System.Threading;
using System.Threading.Tasks;
using Common.Abstractions.Data;
using Factories;

public class LockRepository(IMySqlConnectionFactory factory) : BaseRepository(factory), ILockRepository
{
    public async Task<bool> CreateLockAsync(string key, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT IGNORE INTO locks VALUES (@key)";

        var rows = await ExecuteAsync(sql, new { key }, cancellationToken);

        return rows == 1;
    }

    public async Task<bool> DeleteLockAsync(string key, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM locks WHERE lockKey = @key";

        var rows = await ExecuteAsync(sql, new { key }, cancellationToken);

        return rows == 1;
    }
}
