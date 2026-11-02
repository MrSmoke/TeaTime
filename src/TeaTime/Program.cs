namespace TeaTime;

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Slack;

public static class Program
{
    // todo: Move to config
    public const string ContactEmail = "contact@runteatime.com";

    public static readonly string Version =
        typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
        throw new InvalidOperationException("Failed to get version info");

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure host
        builder.Configuration.AddEnvironmentVariables("TEATIME_");

        // Setup logging
        builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.OpenTelemetry(options => options.ResourceAttributes = new Dictionary<string, object>
            {
                ["service.name"] = context.HostingEnvironment.ApplicationName,
                ["service.version"] = Version
            }));

        // Configure OpenTelemetry
        ConfigureOpenTelemetry(builder);

        // Register services
        builder.Services.AddHealthChecks();

        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        // Build the app
        var app = builder.Build();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseHealthChecks("/_health");

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
        app.UseSlack();

        // Log our version for startup
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("TeaTime");
        logger.LogInformation("TeaTime - {Version}", Version);

        // Run
        app.Run();
    }

    private static void ConfigureOpenTelemetry(WebApplicationBuilder builder)
    {
        var otel = builder.Services.AddOpenTelemetry();

        // Configure OpenTelemetry Resources with the application name
        otel.ConfigureResource(resource =>
        {
            resource.AddService(serviceName: builder.Environment.ApplicationName, serviceVersion: Version);
        });

        otel.UseOtlpExporter();

        // Metrics
        otel.WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddMeter("MySqlConnector");
        });

        // Tracing
        otel.WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    // Remove health check endpoint from otel
                    options.Filter = context => !context.Request.Path.StartsWithSegments("/_health");
                })
                .AddHttpClientInstrumentation()
                .AddSource("MySqlConnector");
        });
    }
}
