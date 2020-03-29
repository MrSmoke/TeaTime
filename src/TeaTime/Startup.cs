namespace TeaTime
{
    using System;
    using AutoMapper;
    using Common;
    using Common.Abstractions;
    using Common.Cache;
    using Common.Features.Runs;
    using Common.Features.Runs.Commands;
    using Common.Options;
    using Common.Permissions;
    using Common.Services;
    using Data.MySql;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Slack;

    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger("Startup");

            _logger.LogInformation("TeaTime - {Version}", Program.Version);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var mvcBuilder = services.AddControllersWithViews();

            services.AddTransient<IStartupFilter, StartupActionFilter>();

            RegisterDataLayer(services);

            // add caching
            // if using more than one instance, replace this with redis or something similar
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICache, Cache>();
            services.AddSingleton<ICacheSerializer, SystemTextJsonCacheSerializer>();

            services.AddSlack(mvcBuilder, _configuration.GetSection("slack"));

            services.AddScoped<IEventPublisher, EventPublisher>();

            services.AddSingleton<IRunnerRandomizer, DefaultRunnerRandomizer>();
            services.AddSingleton<IRoomRunLockService, RoomRunLockService>();
            services.AddSingleton<ISystemClock, DefaultSystemClock>();
            services.AddSingleton<IPermissionService, PermissionService>();

            services.AddAutoMapper(typeof(ICommand));

            // Register the pipelines manually so we can define order
            // - Check permissions first
            // - Then run PreProcessors
            // - run the lock behaviour, this should be the last behaviour aside from caching
            // - finally run the cache behaviour. Im putting this last for now just so everything else runs all the time
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(PermissionBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandPreProcessorBehavior<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(CacheBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RunLockBehavior<,>));

            // Register all PreProcessors
            services.AddSingleton<ICommandPreProcessor<EndRunCommand>, RunPreProcessor>();

            services.AddMediatR(typeof(ICommand));

            // Run Lock should be last behavior to run

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePages();
                app.UseStatusCodePagesWithReExecute("/ErrorStatusCode", "?code={0}");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private void RegisterDataLayer(IServiceCollection services)
        {
            var mysqlOptions = _configuration.GetSection("mysql").Get<MySqlConnectionOptions>();
            if (mysqlOptions != null)
            {
                try
                {
                    mysqlOptions.Validate();
                }
                catch (InvalidOptionException ex)
                {
                    _logger.LogError(ex.Message);

                    throw;
                }

                services.AddMySql(mysqlOptions);

                return;
            }

            _logger.LogCritical("No database has been configured");

            throw new Exception("No database has been configured");
        }
    }
}
