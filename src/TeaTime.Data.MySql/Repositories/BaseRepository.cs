namespace TeaTime.Data.MySql.Repositories;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Factories;
using MySqlConnector;

public abstract class BaseRepository(IMySqlConnectionFactory factory)
{
    protected Task<int> ExecuteAsync(string sql, object obj, CancellationToken cancellationToken = default)
    {
        var command = CreateCommandDefinition(sql, obj, CommandFlags.Buffered, cancellationToken);
        return GetConnection(conn => conn.ExecuteAsync(command));
    }

    protected Task<T?> SingleOrDefaultAsync<T>(string sql, object? obj = null, CancellationToken cancellationToken = default)
    {
        var command = CreateCommandDefinition(sql, obj, cancellationToken: cancellationToken);
        return GetConnection(conn => conn.QuerySingleOrDefaultAsync<T>(command));
    }

    protected Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? obj = null, CancellationToken cancellationToken = default)
    {
        var command = CreateCommandDefinition(sql, obj, cancellationToken: cancellationToken);
        return GetConnection(conn => conn.QueryFirstOrDefaultAsync<T>(command));
    }

    protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object? obj = null, CancellationToken cancellationToken = default)
    {
        var command = CreateCommandDefinition(sql, obj, cancellationToken: cancellationToken);
        return GetConnection(conn => conn.QueryAsync<T>(command));
    }

    private static CommandDefinition CreateCommandDefinition(string sql,
        object? obj = null,
        CommandFlags commandFlags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return new CommandDefinition(commandText: sql,
            parameters: obj,
            flags: commandFlags,
            cancellationToken: cancellationToken);
    }

    private async Task<T> GetConnection<T>(Func<MySqlConnection, Task<T>> func)
    {
        await using var conn = factory.GetConnection();
        return await func(conn);
    }
}
