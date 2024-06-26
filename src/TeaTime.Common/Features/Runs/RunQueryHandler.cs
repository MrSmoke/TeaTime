namespace TeaTime.Common.Features.Runs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using MediatR;
    using Models.Data;
    using Queries;
    using Rooms.Queries;

    public class RunQueryHandler :
        IRequestHandler<GetCurrentRunQuery, Run?>,
        IRequestHandler<GetRunQuery, Run?>
    {
        private readonly IRunRepository _runRepository;

        public RunQueryHandler(IRunRepository runRepository)
        {
            _runRepository = runRepository;
        }

        public Task<Run?> Handle(GetCurrentRunQuery request, CancellationToken cancellationToken)
        {
            return _runRepository.GetCurrentRunAsync(request.RoomId);
        }

        public Task<Run?> Handle(GetRunQuery request, CancellationToken cancellationToken)
        {
            return _runRepository.GetAsync(request.RunId);
        }
    }
}
