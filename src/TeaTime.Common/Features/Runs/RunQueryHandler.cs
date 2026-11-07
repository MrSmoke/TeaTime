namespace TeaTime.Common.Features.Runs;

using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using MediatR;
using Models.Data;
using Queries;
using Rooms.Queries;

public class RunQueryHandler(IRunRepository runRepository) :
    IRequestHandler<GetCurrentRunQuery, RunEntity?>,
    IRequestHandler<GetRunQuery, RunEntity?>
{
    public Task<RunEntity?> Handle(GetCurrentRunQuery request, CancellationToken cancellationToken)
    {
        return runRepository.GetCurrentRunAsync(request.RoomId, cancellationToken);
    }

    public Task<RunEntity?> Handle(GetRunQuery request, CancellationToken cancellationToken)
    {
        return runRepository.GetAsync(request.RunId, cancellationToken);
    }
}
