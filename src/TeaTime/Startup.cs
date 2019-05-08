namespace TeaTime
{
    using AutoMapper;
    using Common;
    using Common.Abstractions;
    using Common.Features.Runs;
    using Common.Features.Runs.Commands;
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
    using Slack.Configuration;

    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var mvcBuilder = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //TODO: Allow these to be loaded dynamically
            services.AddMySql(Configuration.GetSection("mysql").Get<MySqlConnectionOptions>());
            services.AddSlack(mvcBuilder, Configuration.GetSection("slack"));

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

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMvc();
        }
    }
}
