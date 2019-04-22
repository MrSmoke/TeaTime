namespace TeaTime
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Serilog;

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration(config => config.AddEnvironmentVariables("TEATIME_"))
                .UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));
    }
}
