namespace TeaTime.Data.MySql.Factories;

using System;
using Microsoft.Extensions.Options;
using MySqlConnector;

public class MySqlConnectionFactory(IOptions<MySqlConnectionOptions> options) : IMySqlConnectionFactory
{
    private readonly string _connectionString = BuildConnectionString(options.Value);

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    private static string BuildConnectionString(MySqlConnectionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var builder = new MySqlConnectionStringBuilder
        {
            Server = options.Host,
            Port = options.Port,
            Database = options.Database,
            UseAffectedRows = true,
            DateTimeKind = MySqlDateTimeKind.Utc,
            MinimumPoolSize = 1
        };

        if (!string.IsNullOrWhiteSpace(options.Username))
            builder.UserID = options.Username;

        if (!string.IsNullOrWhiteSpace(options.Password))
            builder.Password = options.Password;

        return builder.ConnectionString;
    }
}
