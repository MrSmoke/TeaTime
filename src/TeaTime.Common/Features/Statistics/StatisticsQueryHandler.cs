namespace TeaTime.Common.Features.Statistics;

using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using MediatR;
using Models;
using Queries;

public class StatisticsQueryHandler(
    IStatisticsRepository statisticsRepository)
    : IRequestHandler<GetGlobalTotalsQuery, GlobalTotals>
{
    public async Task<GlobalTotals> Handle(GetGlobalTotalsQuery request, CancellationToken cancellationToken)
    {
        var ordersTask = statisticsRepository.CountGlobalOrdersMadeAsync(cancellationToken);
        var runsTask = statisticsRepository.CountGlobalEndedRunsAsync(cancellationToken);

        return new GlobalTotals
        {
            OrdersMade = await ordersTask,
            RunsEnded = await runsTask
        };
    }
}
