namespace TeaTime.Data.MySql
{
    using System;
    using Common;
    using Evolve;
    using Microsoft.Extensions.Logging;

    public class MySqlServerVerificationStartupAction : IStartupAction
    {
        private readonly IMySqlConnectionFactory _connectionFactory;
        private readonly ILogger<MySqlServerVerificationStartupAction> _logger;

        public string Name => "MySQL Server Verification";

        public MySqlServerVerificationStartupAction(IMySqlConnectionFactory connectionFactory, ILogger<MySqlServerVerificationStartupAction> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public void Execute()
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();

                    _logger.LogInformation("MySQL connectivity OK!");

                    var evolve = new Evolve(conn, msg => _logger.LogInformation(msg))
                    {
                        EmbeddedResourceAssemblies = new[] {typeof(MySqlServerVerificationStartupAction).Assembly},
                        IsEraseDisabled = true
                    };

                    evolve.Migrate();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MySQL");

                throw;
            }
        }


    }
}
