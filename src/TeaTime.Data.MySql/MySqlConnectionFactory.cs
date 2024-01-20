namespace TeaTime.Data.MySql
{
    using System;
    using MySqlConnector;

    public class MySqlConnectionFactory : IMySqlConnectionFactory
    {
        private readonly string _connectionString;

        public MySqlConnectionFactory(MySqlConnectionOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _connectionString = GetConnectionString(options);
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        private static string GetConnectionString(MySqlConnectionOptions options)
        {
            static string GetOption(string key, string? value)
                => string.IsNullOrWhiteSpace(value) ? string.Empty : key + "=" + value + ";";

            return string.Concat(
                GetOption("host", options.Host),
                GetOption("port", options.Port.ToString()),
                GetOption("username", options.Username),
                GetOption("password", options.Password),
                GetOption("database", options.Database),
                "UseAffectedRows=true;",
                "DateTimeKind=utc;"
            );
        }
    }
}
