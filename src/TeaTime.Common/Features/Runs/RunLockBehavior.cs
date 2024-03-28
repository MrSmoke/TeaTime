namespace TeaTime.Common.Features.Runs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Commands;
    using Exceptions;
    using MediatR;

    public class RunLockBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse> where TCommand : notnull
    {
        private readonly IRoomRunLockService _lockService;

        public RunLockBehavior(IRoomRunLockService lockService)
        {
            _lockService = lockService;
        }

        public async Task<TResponse> Handle(TCommand request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            await ProcessAsync(request, cancellationToken);

            return await next();
        }

        private Task ProcessAsync(TCommand request, CancellationToken cancellationToken)
        {
            return request switch
            {
                StartRunCommand startRunCommand => ProcessAsync(startRunCommand, cancellationToken),
                EndRunCommand endRunCommand => ProcessAsync(endRunCommand, cancellationToken),
                _ => Task.CompletedTask
            };
        }

        private async Task ProcessAsync(StartRunCommand request, CancellationToken cancellationToken)
        {
            //try to create the run lock
            var created = await _lockService.CreateLockAsync(request.RoomId);
            if (!created)
                throw new RunStartException("There is already an active run in this room",
                    RunStartException.RunStartExceptionReason.ExistingActiveRun);

            //run lock created, so everything is ok
        }

        private async Task ProcessAsync(EndRunCommand request, CancellationToken cancellationToken)
        {
            //delete lock
            if (await _lockService.DeleteLockAsync(request.Run.RoomId))
                return;

            throw new RunEndException("There is no active run in this room",
                RunEndException.RunEndExceptionReason.NoActiveRun);
        }
    }
}
