namespace TeaTime.Data.MySql.Repositories;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Factories;
using MySqlConnector;

public abstract class BaseRepository(IMySqlConnectionFactory factory)
{
    protected Task<int> ExecuteAsync(string sql, object obj)
    {
        return GetConnection(conn => conn.ExecuteAsync(sql, obj));
    }

    protected Task<T?> SingleOrDefaultAsync<T>(string sql, object? obj = null)
    {
        return GetConnection(conn => conn.QuerySingleOrDefaultAsync<T>(sql, obj));
    }

    protected Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? obj = null)
    {
        return GetConnection(conn => conn.QueryFirstOrDefaultAsync<T>(sql, obj));
    }

    protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object? obj = null)
    {
        return GetConnection(conn => conn.QueryAsync<T>(sql, obj));
    }

    private async Task<T> GetConnection<T>(Func<MySqlConnection, Task<T>> func)
    {
        await using var conn = factory.GetConnection();

        return await func(conn);
    }
}
