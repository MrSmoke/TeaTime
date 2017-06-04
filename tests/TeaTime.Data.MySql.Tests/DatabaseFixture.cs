namespace TeaTime.Data.MySql.Tests
{
    using System;
    using Dapper;
    using global::MySql.Data.MySqlClient;

    public class DatabaseFixture :IDisposable
    {
        public ConnectionFactory ConnectionFactory;

        public DatabaseFixture()
        {
            ConnectionFactory = new ConnectionFactory(TestConfig.ConnectionString);

            using (var conn = new MySqlConnection(TestConfig.ConnectionString))
            {
                conn.Execute($"create schema if not exits {TestConfig.Schema}");
            }
        }

        public void Dispose()
        {
        }
    }
}
