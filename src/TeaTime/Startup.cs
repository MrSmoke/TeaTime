namespace TeaTime
{
    using System;
    using Common;
    using Common.Abstractions;
    using Common.Cache;
    using Common.Features.Runs;
    using Common.Features.Runs.Commands;
    using Common.Permissions;
    using Common.Services;
    using Data.MySql;
    using MediatR;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Slack;

    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var mvcBuilder = services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.AddTransient<IStartupFilter, StartupActionFilter>();

            // Register our data services
            RegisterDataLayer(services);

            // add caching
            // if using more than one instance, replace this with redis or something similar
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICache, Cache>();
            services.AddSingleton<ICacheSerializer, SystemTextJsonCacheSerializer>();

            // Add Slack support
            services.AddSlack(mvcBuilder, _configuration.GetSection("slack"));

            // Core
            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddSingleton<IRunnerRandomizer, DefaultRunnerRandomizer>();
            services.AddSingleton<IRoomRunLockService, RoomRunLockService>();
            services.AddSingleton<ISystemClock, DefaultSystemClock>();
            services.AddSingleton<IPermissionService, PermissionService>();
            services.AddSingleton<IUrlGenerator, UrlGenerator>();
            services.Configure<UrlGeneratorOptions>(_configuration);

            // Register the pipelines manually so we can define order
            // - Check permissions first
            // - Then run PreProcessors
            // - Finally, run the cache layer before hitting the actual implementations
            // - Run the lock behaviour, this should be the last behaviour aside from caching
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(PermissionBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandPreProcessorBehavior<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(CacheBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RunLockBehavior<,>));

            // Register all PreProcessors
            services.AddSingleton<ICommandPreProcessor<EndRunCommand>, RunPreProcessor>();

            services.AddMediatR(o =>
            {
                o.RegisterServicesFromAssemblyContaining<ICommand>();
            });
        }

        private void RegisterDataLayer(IServiceCollection services)
        {
            var mysqlOptions = _configuration.GetSection("mysql").Get<MySqlConnectionOptions>();
            if (mysqlOptions == null)
                throw new Exception("No database has been configured");

            mysqlOptions.Validate();

            services.AddMySql(mysqlOptions);
        }
    }
}
