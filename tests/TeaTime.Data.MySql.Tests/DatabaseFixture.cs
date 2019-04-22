namespace TeaTime.Data.MySql.Tests
{
    using System;
    using Dapper;
    using global::MySql.Data.MySqlClient;

    public class DatabaseFixture : IDisposable
    {
        public readonly IMySqlConnectionFactory ConnectionFactory;

        public DatabaseFixture()
        {
            ConnectionFactory = new MySqlConnectionFactory(TestConfig.ConnectionOptions);

            using (var conn = ConnectionFactory.GetConnection())
            {
                conn.Execute($"create schema if not exits {TestConfig.Schema}");
            }
        }

        public void Dispose()
        {
        }
    }
}
