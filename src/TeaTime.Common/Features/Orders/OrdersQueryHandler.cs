namespace TeaTime.Common.Features.Orders;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using MediatR;
using Models.Data;
using Queries;

public class OrdersQueryHandler(
    IOrderRepository orderRepository) :
    IRequestHandler<GetUserOrderQuery, OrderEntity?>
{
    public async Task<OrderEntity?> Handle(GetUserOrderQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetOrdersAsync(request.RunId, cancellationToken);

        return orders.FirstOrDefault(o => o.UserId == request.UserId);
    }
}
