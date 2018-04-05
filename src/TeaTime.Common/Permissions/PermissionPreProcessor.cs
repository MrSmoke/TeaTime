namespace TeaTime.Common.Permissions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using MediatR.Pipeline;

    public class PermissionPreProcessor<TCommand> : IRequestPreProcessor<TCommand>
    {
        private readonly IPermissionService _permissionService;

        public PermissionPreProcessor(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public Task Process(TCommand request, CancellationToken cancellationToken)
        {
            if (request is IUserCommand command)
            {
                return _permissionService.CheckAsync(command);
            }

            return Task.CompletedTask;
        }
    }
}
