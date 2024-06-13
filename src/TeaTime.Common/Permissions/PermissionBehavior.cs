namespace TeaTime.Common.Permissions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Exceptions;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class PermissionBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse> where TCommand : notnull
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionBehavior<TCommand, TResponse>> _logger;

        public PermissionBehavior(IPermissionService permissionService,
            ILogger<PermissionBehavior<TCommand, TResponse>> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TCommand request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is IUserCommand command)
            {
                _logger.LogDebug("Checking permission for command {Command}", CommandTypeName);

                var result = await _permissionService.CheckAsync(command);

                Handle(result);
            }
            else
            {
                _logger.LogDebug("Skipping permission check for command {Command}", CommandTypeName);
            }

            return await next();
        }

        private void Handle(PermissionCheckResult result)
        {
            if (result.Success)
            {
                _logger.LogDebug("Permission check succeeded for command {Command}", CommandTypeName);

                return;
            }

            _logger.LogDebug("Permission check failed for command {Command} with message {Message}",
                CommandTypeName, result.Message);

            throw new PermissionException(result.Message!);
        }

        private static string CommandTypeName => typeof(TCommand).Name;
    }
}
