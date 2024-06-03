namespace TeaTime.Common.Permissions
{
    using System;
    using System.Threading.Tasks;
    using Abstractions;

    public class PermissionService : IPermissionService
    {
        private readonly IServiceProvider _serviceProvider;

        public PermissionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<PermissionCheckResult> CheckAsync<T>(T request) where T : IUserCommand
        {
            if (_serviceProvider.GetService(typeof(IPermissionCheck<T>)) is IPermissionCheck<T> check)
                return check.CheckAsync(request);

            return Task.FromResult(PermissionCheckResult.Ok());
        }

        public Task CheckAsync<TQuery, TResponse>(TQuery query) where TQuery : IUserQuery<TResponse>
        {
            throw new NotImplementedException();
        }
    }
}
