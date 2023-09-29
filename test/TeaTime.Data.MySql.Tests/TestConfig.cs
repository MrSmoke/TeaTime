namespace TeaTime.Data.MySql.Tests
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;

    public static class TestConfig
    {
        public static MySqlConnectionOptions ConnectionOptions => ConfigBuilder.Value
            .GetSection("mysql")
            .Get<MySqlConnectionOptions>() ?? throw new Exception("Failed to load mysql config");

        public static string Schema => ConfigBuilder.Value["schema"] ?? throw new Exception("Schema not set in config");

        private static Lazy<IConfiguration> ConfigBuilder { get; } = new(
            () => new ConfigurationBuilder()
                .SetBasePath(GetCodeRootPath())
                .AddJsonFile("config.json")
                .Build());

        private static string GetCodeRootPath()
        {
            var currentAssembly = typeof(TestConfig).GetTypeInfo().Assembly;

            var directory = new Uri(currentAssembly.Location).LocalPath;

            return directory;
        }
    }
}
