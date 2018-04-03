namespace TeaTime.Common.Permissions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using MediatR.Pipeline;

    public class PermissionPreProcessor<TCommand> : IRequestPreProcessor<TCommand> where TCommand : IUserCommand
    {
        private readonly IPermissionService _permissionService;

        public PermissionPreProcessor(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public Task Process(TCommand request, CancellationToken cancellationToken)
        {
            return _permissionService.CheckAsync(request);
        }
    }
}
