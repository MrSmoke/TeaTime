namespace TeaTime.Data.MySql.Tests;

using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class DatabaseFixture : IAsyncLifetime
{
    public readonly IMySqlConnectionFactory
        ConnectionFactory = new MySqlConnectionFactory(TestConfig.ConnectionOptions);

    public async Task InitializeAsync()
    {
        await using var conn = ConnectionFactory.GetConnection();

        if (string.IsNullOrEmpty(TestConfig.ConnectionOptions.Database))
            throw new Exception("No database defined in config");

        // Delete old tables to start fresh
        const string sql = "SET FOREIGN_KEY_CHECKS = 0;" +
                           "drop table if exists changelog;" +
                           "drop table if exists hashes;" +
                           "drop table if exists ids64;" +
                           "drop table if exists illmakes;" +
                           "drop table if exists links;" +
                           "drop table if exists locks;" +
                           "drop table if exists option_groups;" +
                           "drop table if exists options;" +
                           "drop table if exists orders;" +
                           "drop table if exists rooms;" +
                           "drop table if exists run_results;" +
                           "drop table if exists runs;" +
                           "drop table if exists users;" +
                           "SET FOREIGN_KEY_CHECKS = 1";

        await conn.ExecuteAsync(sql);

        // Do database migrations against new schema
        // todo: We should't be calling MySqlServerVerificationStartupAction
        var migration = new MySqlServerVerificationStartupAction(ConnectionFactory,
            new NullLogger<MySqlServerVerificationStartupAction>());

        migration.Execute();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
