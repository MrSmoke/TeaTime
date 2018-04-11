namespace TeaTime.Common.Permissions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Exceptions;
    using MediatR.Pipeline;

    public class PermissionPreProcessor<TCommand> : IRequestPreProcessor<TCommand>
    {
        private readonly IPermissionService _permissionService;

        public PermissionPreProcessor(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task Process(TCommand request, CancellationToken cancellationToken)
        {
            if (request is IUserCommand command)
            {
                var result = await _permissionService.CheckAsync(command).ConfigureAwait(false);

                Handle(result);
            }
        }

        private static void Handle(PermisionCheckResult result)
        {
            if (result.Success)
                return;

            throw new PermissionException(result.Message);
        }
    }
}
