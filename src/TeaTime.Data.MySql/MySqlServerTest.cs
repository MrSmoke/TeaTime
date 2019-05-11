namespace TeaTime.Data.MySql
{
    using System;
    using Common;
    using Microsoft.Extensions.Logging;

    public class MySqlServerTestStartupAction : IStartupAction
    {
        private readonly IMySqlConnectionFactory _connectionFactory;
        private readonly ILogger<MySqlServerTestStartupAction> _logger;

        public string Name => "MySQL Server Connection Test";

        public MySqlServerTestStartupAction(IMySqlConnectionFactory connectionFactory, ILogger<MySqlServerTestStartupAction> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public void Execute()
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                    conn.Open();

                _logger.LogInformation("MySQL connectivity OK!");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MySQL");

                throw;
            }
        }
    }
}
