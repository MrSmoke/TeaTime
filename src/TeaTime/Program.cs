namespace TeaTime;

using System;
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
    public static readonly string ContactEmail = "contact@runteatime.com";

    public static readonly string Version =
        typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
        throw new InvalidOperationException("Failed to get version info");

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure host
        builder.Configuration.AddEnvironmentVariables("TEATIME_");
        builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

        // Configure OpenTelemetry
        ConfigureOpenTelemetry(builder);

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
        app.UseSlack();

        // Log our version for startup
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("TeaTime");
        logger.LogInformation("TeaTime - {Version}", Version);

        // Run
        app.Run();
    }

    private static void ConfigureOpenTelemetry(WebApplicationBuilder builder)
    {
        var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
        var otel = builder.Services.AddOpenTelemetry();

        // Configure OpenTelemetry Resources with the application name
        otel.ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName));

        // Setup OtlpExporer
        otel.UseOtlpExporter();

        // Metrics
        otel.WithMetrics(metrics =>
        {
            // Add meters
            metrics.AddAspNetCoreInstrumentation()
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                .AddMeter("MySqlConnector");

            // Setup export
            if (builder.Environment.IsDevelopment())
            {
                metrics.AddConsoleExporter();
            }
        });

        // Tracing
        otel.WithTracing(tracing =>
        {
            // Setup sources
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            tracing.AddSource("MySqlConnector");

            // Setup export
            if (builder.Environment.IsDevelopment())
            {
                tracing.AddConsoleExporter();
            }
        });
    }
}
