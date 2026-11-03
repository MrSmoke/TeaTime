namespace TeaTime.Common.Permissions;

using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

public class PermissionBehavior<TCommand, TResponse>(
    IPermissionService permissionService,
    ILogger<PermissionBehavior<TCommand, TResponse>> logger)
    : IPipelineBehavior<TCommand, TResponse>
    where TCommand : IUserCommand
{
    public async Task<TResponse> Handle(TCommand request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Checking permission for command {Command}", typeof(TCommand).Name);

        var result = await permissionService.CheckAsync(request, cancellationToken);
        if (!result.Success)
        {
            logger.LogDebug("Permission check failed for command {Command} with message {Message}",
                typeof(TCommand).Name, result.Message);

            throw new PermissionException(result.Message ?? "Permission denied");
        }

        logger.LogDebug("Permission check succeeded for command {Command}", typeof(TCommand).Name);

        return await next(cancellationToken);
    }
}
