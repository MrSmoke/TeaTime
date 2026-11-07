namespace TeaTime.Common.Abstractions;

using System.Threading;
using System.Threading.Tasks;
using Permissions;

public interface IPermissionService
{
    Task<PermissionCheckResult> CheckAsync<T>(T request, CancellationToken cancellationToken = default)
        where T : IUserCommand;
    Task CheckAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IUserQuery<TResponse>;
}
