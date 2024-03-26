namespace TeaTime.Common.Features.Runs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using MediatR;
    using Models.Data;
    using Queries;

    public class RunQueryHandler :
        IRequestHandler<GetCurrentRunQuery, Run?>,
        IRequestHandler<GetRunQuery, Run?>,
        IRequestHandler<GetRunsByRoomId, IEnumerable<Run>>
    {
        private readonly IRunRepository _runRepository;

        public RunQueryHandler(IRunRepository runRepository)
        {
            _runRepository = runRepository;
        }

        public async Task<Run?> Handle(GetCurrentRunQuery request, CancellationToken cancellationToken)
        {
            var runs = await _runRepository.GetManyByRoomId(request.RoomId, ended: false, limit: 1);
            return runs.FirstOrDefault();
        }

        public Task<Run?> Handle(GetRunQuery request, CancellationToken cancellationToken)
        {
            return _runRepository.GetAsync(request.RunId);
        }

        public Task<IEnumerable<Run>> Handle(GetRunsByRoomId request, CancellationToken cancellationToken)
        {
            return _runRepository.GetManyByRoomId(request.RoomId, limit: request.Limit);
        }
    }
}
