namespace TeaTime.Common.Features.Orders;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Exceptions;
using MediatR;
using Models.Data;
using Models.Domain;
using Queries;

public class OrdersQueryHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IOptionsRepository optionsRepository,
    IRunRepository runRepository) :
    IRequestHandler<GetRunOrdersQuery, IEnumerable<OrderModel>>,
    IRequestHandler<GetUserOrderQuery, OrderEntity?>
{
    public async Task<IEnumerable<OrderModel>> Handle(GetRunOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var run = await runRepository.GetAsync(request.RunId, cancellationToken);
        if (run is null)
            throw new TeaTimeException($"Unknown RunId {request.RunId}");

        var orders = (await orderRepository.GetOrdersAsync(run.Id, cancellationToken)).ToList();
        if (orders.Count == 0)
            return [];

        var users = userRepository.GetManyAsync(orders.Select(o => o.UserId), cancellationToken);
        var options = optionsRepository.GetManyAsync(orders.Select(o => o.OptionId), cancellationToken);

        var userDict = (await users).ToDictionary(k => k.Id);
        var optionsDict = (await options).ToDictionary(k => k.Id);

        return orders
            .Select(order => new OrderModel
            {
                Id = order.Id,
                CreatedDate = order.CreatedDate,
                Run = run,
                User = userDict.GetValueOrDefault(order.UserId),
                Option = optionsDict.GetValueOrDefault(order.OptionId)
            })
            // Order by users display name for consistent ordering
            .OrderBy(o => o.User?.DisplayName);
    }

    public async Task<OrderEntity?> Handle(GetUserOrderQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetOrdersAsync(request.RunId, cancellationToken);

        return orders.FirstOrDefault(o => o.UserId == request.UserId);
    }
}
