namespace TeaTime.Data.MySql.Tests
{
    using System;
    using Microsoft.Extensions.Configuration;

    public static class TestConfig
    {
        public static MySqlConnectionOptions ConnectionOptions => ConfigBuilder.Value
            .GetSection("mysql")
            .Get<MySqlConnectionOptions>() ?? throw new Exception("Failed to load mysql config");

        private static Lazy<IConfiguration> ConfigBuilder { get; } = new(
            () => new ConfigurationBuilder()
                .AddJsonFile("config.json", optional: false)
                .Build());
    }
}
