namespace TeaTime.Common.Features.Orders
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using AutoMapper;
    using Commands;
    using Events;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Models.Data;

    public class OrderCommandHandler :
        IRequestHandler<CreateOrderCommand>,
        IRequestHandler<UpdateOrderOptionCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ISystemClock _clock;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<OrderCommandHandler> _logger;

        public OrderCommandHandler(IOrderRepository orderRepository,
            ISystemClock clock,
            IMapper mapper,
            IEventPublisher eventPublisher,
            ILogger<OrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _clock = clock;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<CreateOrderCommand, Order>(request);
            order.CreatedDate = _clock.UtcNow();

            await _orderRepository.CreateAsync(order);

            var evt = _mapper.Map<Order, OrderPlacedEvent>(order);
            evt.State = request.State;

            await _eventPublisher.PublishAsync(evt);

            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateOrderOptionCommand request, CancellationToken cancellationToken)
        {
            var existing = await _orderRepository.GetAsync(request.OrderId);

            if (existing is null)
            {
                _logger.LogWarning("Failed to get existing order {OrderId}", request.OrderId);
                return Unit.Value;
            }

            if (existing.OptionId == request.OptionId)
                return Unit.Value;

            //store the previous id for the event
            var previousOptionId = existing.OptionId;

            //update
            existing.OptionId = request.OptionId;
            await _orderRepository.UpdateAsync(existing);

            //create event
            var evt = new OrderOptionChangedEvent(
                OrderId: request.OrderId,
                UserId: request.UserId,
                PreviousOptionId: previousOptionId,
                OptionId: request.OptionId
            );

            await _eventPublisher.PublishAsync(evt);

            return Unit.Value;
        }
    }
}
