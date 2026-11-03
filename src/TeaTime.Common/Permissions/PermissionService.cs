namespace TeaTime.Common.Permissions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Microsoft.Extensions.Logging;

public class PermissionService(IServiceProvider serviceProvider, ILogger<PermissionService> logger)
    : IPermissionService
{
    public Task<PermissionCheckResult> CheckAsync<T>(T request, CancellationToken cancellationToken = default)
        where T : IUserCommand
    {
        if (serviceProvider.GetService(typeof(IPermissionCheck<T>)) is IPermissionCheck<T> check)
        {
            logger.LogDebug("Found permission check {Check} for {Request}", check.GetType().Name, typeof(T).Name);
            return check.CheckAsync(request, cancellationToken);
        }

        logger.LogDebug("No permission check registered for {Request}", typeof(T).Name);
        return Task.FromResult(PermissionCheckResult.Ok());
    }

    public Task CheckAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IUserQuery<TResponse>
    {
        throw new NotImplementedException();
    }
}
