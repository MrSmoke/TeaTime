namespace TeaTime
{
    using System;
    using AutoMapper;
    using Common;
    using Common.Abstractions;
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
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var mvcBuilder = services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddTransient<IStartupFilter, StartupActionFilter>();

            RegisterDataLayer(services);

            services.AddSlack(mvcBuilder, _configuration.GetSection("slack"));

            services.AddScoped<IEventPublisher, EventPublisher>();

            services.AddSingleton<IRunnerRandomizer, DefaultRunnerRandomizer>();
            services.AddSingleton<IRoomRunLockService, RoomRunLockService>();
            services.AddSingleton<ISystemClock, DefaultSystemClock>();
            services.AddSingleton<IPermissionService, PermissionService>();

            services.AddAutoMapper(typeof(ICommand));

            // Register manually so we can define order

            // - Check permissions first
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(PermissionBehavior<,>));

            // - Run pre processors
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandPreProcessorBehavior<,>));
            services.AddSingleton<ICommandPreProcessor<EndRunCommand>, RunPreProcessor>();

            services.AddMediatR(typeof(ICommand));

            // Run Lock should be last behavior to run
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RunLockBehavior<,>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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
            app.UseMvc();
        }
    }
}
