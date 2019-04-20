namespace TeaTime.Common.Features.Runs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Commands;
    using Exceptions;

    public class RunLockPreProcessor :
        ICommandPreProcessor<StartRunCommand>,
        ICommandPreProcessor<EndRunCommand>
    {
        private readonly IRoomRunLockService _lockService;

        public RunLockPreProcessor(IRoomRunLockService lockService)
        {
            _lockService = lockService;
        }

        public async Task ProcessAsync(StartRunCommand request, CancellationToken cancellationToken)
        {
            //try to create the run lock
            var created = await _lockService.CreateLockAsync(request.RoomId).ConfigureAwait(false);
            if (!created)
                throw new RunStartException("There is already an active run in this room", RunStartException.RunStartExceptionReason.ExistingActiveRun);

            //run lock created, so everything is ok
        }

        public async Task ProcessAsync(EndRunCommand request, CancellationToken cancellationToken)
        {
            //delete lock
            var deleted = await _lockService.DeleteLockAsync(request.RoomId).ConfigureAwait(false);
            if (!deleted)
                throw new RunEndException("There is no active run in this room", RunEndException.RunEndExceptionReason.NoActiveRun);
        }
    }
}
