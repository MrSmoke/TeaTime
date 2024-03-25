namespace TeaTime.Data.MySql;

using System;
using Common;
using EvolveDb;
using Factories;
using Microsoft.Extensions.Logging;

public class MySqlServerVerificationStartupAction(
    IMySqlConnectionFactory connectionFactory,
    ILogger<MySqlServerVerificationStartupAction> logger)
    : IStartupAction
{
    public string Name => "MySQL Server Verification";

    public void Execute()
    {
        try
        {
            using var conn = connectionFactory.GetConnection();

            conn.Open();

            logger.LogInformation("MySQL connectivity OK!");

            var evolve = new Evolve(conn, msg => logger.LogInformation(msg))
            {
                EmbeddedResourceAssemblies = new[] { typeof(MySqlServerVerificationStartupAction).Assembly },
                IsEraseDisabled = true
            };

            evolve.Migrate();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to MySQL");

            throw;
        }
    }
}
