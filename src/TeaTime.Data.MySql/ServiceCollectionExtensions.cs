namespace TeaTime.Data.MySql
{
    using Common.Abstractions;
    using Common.Abstractions.Data;
    using Microsoft.Extensions.DependencyInjection;
    using Repositories;

    public static class ServiceCollectionExtensions
    {
        public static void AddMySql(this IServiceCollection services, string connectionString)
        {
            var factory = new ConnectionFactory(connectionString);

            services.AddSingleton(factory);

            services.AddSingleton<ILinkRepository, LinkRepository>();
            services.AddSingleton<IRoomRepository, RoomRepository>();
            services.AddSingleton<IRunRepository, RunRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<IOptionsRepository, OptionsRepository>();

            services.AddSingleton<IIdGenerator<long>, MySqlIdGenerator>();
        }
    }
}
