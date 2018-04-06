namespace TeaTime.Common.Features.Runs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using MediatR;
    using Models.Domain;
    using Queries;

    public class OrdersQueryHandler : IRequestHandler<GetRunOrdersQuery, IEnumerable<OrderModel>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOptionsRepository _optionsRepository;
        private readonly IRunRepository _runRepository;

        public OrdersQueryHandler(IOrderRepository orderRepository, IUserRepository userRepository, IOptionsRepository optionsRepository, IRunRepository runRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _optionsRepository = optionsRepository;
            _runRepository = runRepository;
        }

        public async Task<IEnumerable<OrderModel>> Handle(GetRunOrdersQuery request, CancellationToken cancellationToken)
        {
            var run = _runRepository.GetAsync(request.RunId);

            var orders = (await _orderRepository.GetOrdersAsync(request.RunId).ConfigureAwait(false)).ToList();

            var users = _userRepository.GetManyAsync(orders.Select(o => o.UserId)).ConfigureAwait(false);
            var options = _optionsRepository.GetManyAsync(orders.Select(o => o.OptionId)).ConfigureAwait(false);

            var userDict = (await users).ToDictionary(k => k.Id);
            var optionsDict = (await options).ToDictionary(k => k.Id);

            var models = new List<OrderModel>();
            foreach (var order in orders)
            {
                models.Add(new OrderModel
                {
                    Id = order.Id,
                    CreatedDate = order.CreatedDate,
                    Run = await run.ConfigureAwait(false),
                    User = userDict[order.UserId],
                    Option = optionsDict[order.OptionId]
                });
            }

            return models;
        }
    }
}
