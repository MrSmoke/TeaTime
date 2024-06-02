namespace TeaTime.Slack
{
    using System;
    using System.Reflection;
    using Client;
    using CommandRouter.Integration.AspNetCore.Extensions;
    using Common.Features.Orders.Events;
    using Common.Features.Runs.Events;
    using EventHandlers;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
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

            // General required services
            services.TryAddSingleton(TimeProvider.System);
            services.AddTransient<IStartupFilter, SlackStartupFilter>();

            // General slack services
            services.AddHttpClient<ISlackApiClient, SlackApiClient>();
            services.AddScoped<ISlackService, SlackService>();

            // OAuth
            services.AddOptions<SlackOAuthOptions>()
                .Bind(configuration.GetSection("oauth"))
                .ValidateOnStart();
            services.AddSingleton<IValidateOptions<SlackOAuthOptions>, SlackOAuthOptions>();
            services.AddScoped<ISlackAuthenticationService, SlackAuthenticationService>();

            // Request verification
            services.AddOptions<SignedSecretsRequestVerifierOptions>()
                .Bind(configuration.GetSection("requestVerification"))
                .ValidateOnStart();
            services.AddSingleton<IValidateOptions<SignedSecretsRequestVerifierOptions>, SignedSecretsRequestVerifierOptions>();
            services.AddSingleton<ISlackRequestVerifier, SignedSecretsRequestVerifier>();

            services.AddTransient<INotificationHandler<RunEndedEvent>, RunEndedHandler>();
            services.AddTransient<INotificationHandler<OrderPlacedEvent>, OrderEventHandler>();
            services.AddTransient<INotificationHandler<OrderOptionChangedEvent>, OrderEventHandler>();
        }

        public static void UseSlack(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<SlackVerifyRequestMiddleware>();
        }
    }
}
