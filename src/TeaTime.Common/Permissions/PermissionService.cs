namespace TeaTime.Common.Permissions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;

public class PermissionService(IServiceProvider serviceProvider) : IPermissionService
{
    public Task<PermissionCheckResult> CheckAsync<T>(T request, CancellationToken cancellationToken = default)
        where T : IUserCommand
    {
        if (serviceProvider.GetService(typeof(IPermissionCheck<T>)) is IPermissionCheck<T> check)
            return check.CheckAsync(request, cancellationToken);

        return Task.FromResult(PermissionCheckResult.Ok());
    }

    public Task CheckAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IUserQuery<TResponse>
    {
        throw new NotImplementedException();
    }
}
