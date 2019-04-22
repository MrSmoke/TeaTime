namespace TeaTime.Data.MySql.Tests
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;

    public static class TestConfig
    {
        public static MySqlConnectionOptions ConnectionOptions =>
            ConfigBuilder.Value.GetSection("mysql").Get<MySqlConnectionOptions>();

        public static string Schema => ConfigBuilder.Value["schema"];

        private static Lazy<IConfiguration> ConfigBuilder { get; } = new Lazy<IConfiguration>(
            () => new ConfigurationBuilder()
                .SetBasePath(GetCodeRootPath())
                .AddJsonFile("config.json")
                .Build());

        private static string GetCodeRootPath()
        {
            var currentAssembly = typeof(TestConfig).GetTypeInfo().Assembly;

            var directory = new Uri(currentAssembly.CodeBase).LocalPath;

            return directory;
        }
    }
}
