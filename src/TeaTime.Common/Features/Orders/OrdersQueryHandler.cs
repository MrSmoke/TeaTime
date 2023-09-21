namespace TeaTime.Common.Features.Orders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using MediatR;
    using Models.Data;
    using Models.Domain;
    using Queries;

    public class OrdersQueryHandler :
        IRequestHandler<GetRunOrdersQuery, IEnumerable<OrderModel>>
        , IRequestHandler<GetUserOrderQuery, Order>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOptionsRepository _optionsRepository;
        private readonly IRunRepository _runRepository;

        public OrdersQueryHandler(IOrderRepository orderRepository, IUserRepository userRepository,
            IOptionsRepository optionsRepository, IRunRepository runRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _optionsRepository = optionsRepository;
            _runRepository = runRepository;
        }

        public async Task<IEnumerable<OrderModel>> Handle(GetRunOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var run = _runRepository.GetAsync(request.RunId);

            var orders = (await _orderRepository.GetOrdersAsync(request.RunId)).ToList();
            if (orders.Count == 0)
                return new List<OrderModel>();

            var users = _userRepository.GetManyAsync(orders.Select(o => o.UserId));
            var options = _optionsRepository.GetManyAsync(orders.Select(o => o.OptionId));

            var userDict = (await users).ToDictionary(k => k.Id);
            var optionsDict = (await options).ToDictionary(k => k.Id);

            var models = new List<OrderModel>();
            foreach (var order in orders)
            {
                models.Add(new OrderModel
                {
                    Id = order.Id,
                    CreatedDate = order.CreatedDate,
                    Run = await run,
                    User = userDict[order.UserId],
                    Option = optionsDict[order.OptionId]
                });
            }

            return models;
        }

        public async Task<Order> Handle(GetUserOrderQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetOrdersAsync(request.RunId);

            return orders.FirstOrDefault(o => o.UserId == request.UserId);
        }
    }
}
