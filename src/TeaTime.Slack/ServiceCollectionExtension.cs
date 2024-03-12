namespace TeaTime.Slack
{
    using System.Reflection;
    using Client;
    using CommandRouter.Integration.AspNetCore.Extensions;
    using Common;
    using Common.Features.Orders.Events;
    using Common.Features.Runs.Events;
    using Common.Options;
    using Configuration;
    using EventHandlers;
    using MediatR;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Services;

    public static class ServiceCollectionExtension
    {
        internal static readonly Assembly Assembly = typeof(ServiceCollectionExtension).GetTypeInfo().Assembly;

        public static void AddSlack(this IServiceCollection services, IMvcBuilder mvcBuilder, IConfiguration configuration)
        {
            //Add current assembly controllers
            mvcBuilder.AddApplicationPart(Assembly);

            services.AddCommandRouter();

            services.AddTransient<IStartupFilter, SlackStartupFilter>();

            // Register our options and the options startup validator
            services.AddOptions<SlackOptions>()
                .Bind(configuration)
                .ValidateOnStart();
            services.AddSingleton<IValidateOptions<SlackOptions>, SlackOptionsValidator>();

            services.AddHttpClient<ISlackApiClient, SlackApiClient>();
            services.AddScoped<ISlackAuthenticationService, SlackAuthenticationService>();
            services.AddScoped<ISlackService, SlackService>();
            services.AddSingleton<ISlackMessageVerifier, SlackMessageVerifier>();

            services.AddTransient<INotificationHandler<RunEndedEvent>, RunEndedHandler>();
            services.AddTransient<INotificationHandler<OrderPlacedEvent>, OrderEventHandler>();
            services.AddTransient<INotificationHandler<OrderOptionChangedEvent>, OrderEventHandler>();
        }
    }
}
