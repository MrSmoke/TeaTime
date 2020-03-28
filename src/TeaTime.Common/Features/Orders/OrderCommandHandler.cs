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
    using Models.Data;

    public class OrderCommandHandler :
        IRequestHandler<CreateOrderCommand>,
        IRequestHandler<UpdateOrderOptionCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ISystemClock _clock;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public OrderCommandHandler(IOrderRepository orderRepository, ISystemClock clock, IMapper mapper, IEventPublisher eventPublisher)
        {
            _orderRepository = orderRepository;
            _clock = clock;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<CreateOrderCommand, Order>(request);
            order.CreatedDate = _clock.UtcNow();

            await _orderRepository.CreateAsync(order).ConfigureAwait(false);

            var evt = _mapper.Map<Order, OrderPlacedEvent>(order);
            evt.State = request.State;

            await _eventPublisher.PublishAsync(evt).ConfigureAwait(false);

            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateOrderOptionCommand request, CancellationToken cancellationToken)
        {
            var existing = await _orderRepository.GetAsync(request.OrderId).ConfigureAwait(false);

            if (existing.OptionId == request.OptionId)
                return Unit.Value;

            //store the previous id for the event
            var previousOptionId = existing.OptionId;

            //update
            existing.OptionId = request.OptionId;
            await _orderRepository.UpdateAsync(existing).ConfigureAwait(false);

            //create event
            var evt = _mapper.Map<UpdateOrderOptionCommand, OrderOptionChangedEvent>(request);
            evt.PreviousOptionId = previousOptionId;

            await _eventPublisher.PublishAsync(evt).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
