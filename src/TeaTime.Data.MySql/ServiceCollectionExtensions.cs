namespace TeaTime.Data.MySql
{
    using System;
    using Common;
    using Common.Abstractions;
    using Common.Abstractions.Data;
    using Common.Collections;
    using Microsoft.Extensions.DependencyInjection;
    using Repositories;

    public static class ServiceCollectionExtensions
    {
        public static void AddMySql(this IServiceCollection services, MySqlConnectionOptions options)
        {
            if(options == null)
                throw new ArgumentNullException(nameof(options));

            options.Validate();

            services.AddSingleton<IMySqlConnectionFactory>(new MySqlConnectionFactory(options));

            services.AddSingleton<ILinkRepository, LinkRepository>();
            services.AddSingleton<IRoomRepository, RoomRepository>();
            services.AddSingleton<IRunRepository, RunRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<IOptionsRepository, OptionsRepository>();
            services.AddSingleton<ILockRepository, LockRepository>();
            services.AddSingleton<IIllMakeRepository, IllMakeRepository>();

            services.AddSingleton<IIdGenerator<long>, MySqlIdGenerator>();
            services.AddSingleton<IDistributedHash, MySqlDistributedHash>();

            services.AddTransient<IStartupAction, MySqlServerVerificationStartupAction>();
        }
    }
}
