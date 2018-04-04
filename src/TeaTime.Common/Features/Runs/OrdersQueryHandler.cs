namespace TeaTime.Common.Features.Runs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using MediatR;
    using Models.Data;
    using Queries;

    public class OrdersQueryHandler : IRequestHandler<GetRunOrdersQuery, IEnumerable<Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task<IEnumerable<Order>> Handle(GetRunOrdersQuery request, CancellationToken cancellationToken)
        {
            return _orderRepository.GetOrdersAsync(request.RunId);
        }
    }
}
