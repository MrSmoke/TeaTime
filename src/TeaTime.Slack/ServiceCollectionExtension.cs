namespace TeaTime.Slack
{
    using System.Reflection;
    using CommandRouter.Integration.AspNetCore.Extensions;
    using Common.Features.Runs.Events;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtension
    {
        public static void AddSlack(this IServiceCollection services, IMvcBuilder mvcBuilder)
        {
            var assembly = typeof(ServiceCollectionExtension).GetTypeInfo().Assembly;

            //Add current assembly controllers
            mvcBuilder.AddApplicationPart(assembly);

            services.AddCommandRouter();

            services.AddSingleton<ISlackService, SlackService>();
            services.AddSingleton<IRunEventListener, RunEventListener>();
            services.AddTransient<INotificationHandler<RunEndedEvent>, RunEndedListener>();
        }
    }
}
