namespace TeaTime.Common.Features.Runs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Commands;
    using MediatR.Pipeline;

    public class RunLockProcessor //:
        //IRequestPreProcessor<StartRunCommand>,
        //IRequestPreProcessor<EndRunCommand>
    {
        private readonly IRoomRunLockService _lockService;

        public RunLockProcessor(IRoomRunLockService lockService)
        {
            _lockService = lockService;
        }

        public async Task Process(StartRunCommand request, CancellationToken cancellationToken)
        {
            //try to create the run lock
            var created = await _lockService.CreateLockAsync(request.Id, request.RoomId).ConfigureAwait(false);
            if (!created)
                throw new Exception("Room already has run"); //todo: better exception

            //run lock created, so everything is ok
        }

        public async Task Process(EndRunCommand request, CancellationToken cancellationToken)
        {
            //delete lock
            var deleted = await _lockService.DeleteLockAsync(request.RunId, request.RoomId).ConfigureAwait(false);
            if (!deleted)
                throw new Exception("No current run in room");
        }
    }
}
