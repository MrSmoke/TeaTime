namespace TeaTime;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common;
using Common.Abstractions;
using Common.Cache;
using Common.Features.Orders.Commands;
using Common.Features.Orders.Permissions;
using Common.Features.Runs;
using Common.Features.Runs.Commands;
using Common.Permissions;
using Common.Services;
using Data.MySql;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Extensions.Logging;
using Slack;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        var serilogProvider = new SerilogLoggerProvider(serilogLogger, dispose: true);

        // Registered directly since AddSerilog adds a Trace filter that overrides the Logging section
        builder.Logging.ClearProviders();
        builder.Logging.Services.AddSingleton<ILoggerProvider>(serilogProvider);

        // Register services
        var services = builder.Services;
        var configuration = builder.Configuration;

        ConfigureOpenTelemetry(builder);
        services.AddHealthChecks();

        // Add framework services
        var mvcBuilder = services.AddControllersWithViews();
        services.AddHttpContextAccessor();

        // Register our data services
        services.AddMySql(configuration.GetSection("mysql"));

        // In-memory only until an IDistributedCache such as Redis is registered for multiple instances
        services.AddHybridCache();

        // Add Slack support
        services.AddSlack(mvcBuilder, configuration.GetSection("slack"));

        // Core
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddSingleton<IRunnerRandomizer, DefaultRunnerRandomizer>();
        services.AddSingleton<IRoomRunLockService, RoomRunLockService>();
        services.AddSingleton<IPermissionService, PermissionService>();
        services.AddSingleton<IUrlGenerator, UrlGenerator>();
        services.Configure<UrlGeneratorOptions>(configuration);

        // Register the pipelines manually so we can define order
        // - Check permissions first
        // - Then run PreProcessors
        // - Finally, run the cache layer before hitting the actual implementations
        // - Run the lock behaviour, this should be the last behaviour aside from caching
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(PermissionBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandPreProcessorBehavior<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(CacheBehaviour<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RunLockBehavior<,>));

        // Register permission checks
        services.AddSingleton<IPermissionCheck<DeleteOrderCommand>, OrderPermissions>();
        services.AddSingleton<IPermissionCheck<UpdateOrderOptionCommand>, OrderPermissions>();

        // Register all PreProcessors
        services.AddSingleton<ICommandPreProcessor<EndRunCommand>, RunPreProcessor>();

        services.AddMediatR(o =>
        {
            o.RegisterServicesFromAssemblyContaining<ICommand>();
        });

        // Build the app
        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("TeaTime");

        // Runs before the pipeline is built so migrations finish before any request is served
        RunStartupActions(app, logger);

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
            app.UseStatusCodePagesWithReExecute("/ErrorStatusCode", "?code={0}");
        }

        app.UseRouting();
        app.MapStaticAssets();
        app.MapControllers().WithStaticAssets();
        app.UseSlack();

        // Log our version for startup
        logger.LogInformation("TeaTime - {Version}", Version);

        // Run
        app.Run();
    }

    private static void RunStartupActions(WebApplication app, ILogger logger)
    {
        var actions = app.Services.GetServices<IStartupAction>().ToArray();

        logger.LogDebug("Running {Count} startup actions", actions.Length);

        foreach (var action in actions)
        {
            logger.LogDebug("Running startup action: {StartupActionName}", action.Name);

            action.Execute();
        }
    }

    private static void ConfigureOpenTelemetry(WebApplicationBuilder builder)
    {
        var otel = builder.Services.AddOpenTelemetry();

        // Configure OpenTelemetry Resources with the application name
        otel.ConfigureResource(resource =>
        {
            resource.AddService(serviceName: builder.Environment.ApplicationName, serviceVersion: Version);
            resource.AddAttributes([
                new KeyValuePair<string, object>("deployment.environment.name", builder.Environment.EnvironmentName)
            ]);
        });

        otel.UseOtlpExporter();

        // Logging
        otel.WithLogging(configureBuilder: null, configureOptions: options =>
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
        });

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
