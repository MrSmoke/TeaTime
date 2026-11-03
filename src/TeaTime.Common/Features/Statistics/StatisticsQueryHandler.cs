namespace TeaTime.Common.Features.Statistics;

using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using MediatR;
using Models;
using Queries;

public class StatisticsQueryHandler : IRequestHandler<GetGlobalTotalsQuery, GlobalTotals>
{
    private readonly IStatisticsRepository _statisticsRepository;

    public StatisticsQueryHandler(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public async Task<GlobalTotals> Handle(GetGlobalTotalsQuery request, CancellationToken cancellationToken)
    {
        var ordersTask = _statisticsRepository.CountGlobalOrdersMadeAsync(cancellationToken);
        var runsTask = _statisticsRepository.CountGlobalEndedRunsAsync(cancellationToken);

        return new GlobalTotals
        {
            OrdersMade = await ordersTask,
            RunsEnded = await runsTask
        };
    }
}
