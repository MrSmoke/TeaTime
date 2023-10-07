namespace TeaTime;

using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

public static class Program
{
    public static readonly string Version =
        typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
        throw new InvalidOperationException("Failed to get version info");

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure host
        builder.Configuration.AddEnvironmentVariables("TEATIME_");
        builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

        // Register services
        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        // Build the app
        var app = builder.Build();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseStatusCodePages();
            app.UseStatusCodePagesWithReExecute("/ErrorStatusCode", "?code={0}");
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllers();

        // Log our version for startup
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("TeaTime");
        logger.LogInformation("TeaTime - {Version}", Version);

        // Run
        app.Run();
    }
}
