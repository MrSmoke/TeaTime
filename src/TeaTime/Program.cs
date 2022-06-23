namespace TeaTime;

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

public static class Program
{
    public static readonly string Version;

    static Program()
    {
        Version = typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure host
        builder.Host
            .ConfigureAppConfiguration(config => config.AddEnvironmentVariables("TEATIME_"))
            .UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

        // Register services
        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        // Build the app
        var app = builder.Build();
        startup.Configure(app, app.Environment);

        // Log our version for startup
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("TeaTime");
        logger.LogInformation("TeaTime - {Version}", Program.Version);

        // Run
        app.Run();
    }
}
