namespace TeaTime.Common.Permissions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Exceptions;
    using Microsoft.Extensions.Logging;

    public class PermissionPreProcessor<TCommand> : ICommandPreProcessor<TCommand>
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionPreProcessor<TCommand>> _logger;

        public PermissionPreProcessor(IPermissionService permissionService, ILogger<PermissionPreProcessor<TCommand>> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        public async Task ProcessAsync(TCommand request, CancellationToken cancellationToken)
        {
            if (request is IUserCommand command)
            {
                _logger.LogDebug("Checking permission for command {Command}", CommandTypeName);

                var result = await _permissionService.CheckAsync(command).ConfigureAwait(false);

                Handle(result);
            }
            else
            {
                _logger.LogDebug("Skipping permission check for command {Command}", CommandTypeName);
            }
        }

        private void Handle(PermisionCheckResult result)
        {
            if (result.Success)
            {
                _logger.LogDebug("Permission check succeeded for command {Command}", CommandTypeName);

                return;
            }

            _logger.LogDebug("Permission check failed for command {Command} with message {Message}",
                CommandTypeName, result.Message);

            throw new PermissionException(result.Message);
        }

        private static string CommandTypeName => typeof(TCommand).Name;
    }
}
