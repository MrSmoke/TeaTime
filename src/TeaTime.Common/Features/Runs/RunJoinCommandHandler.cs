namespace TeaTime.Common.Features.Runs
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

    public class RunJoinCommandHandler : IRequestHandler<JoinRunCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ISystemClock _clock;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public RunJoinCommandHandler(IOrderRepository orderRepository, ISystemClock clock, IMapper mapper, IEventPublisher eventPublisher)
        {
            _orderRepository = orderRepository;
            _clock = clock;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(JoinRunCommand request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<JoinRunCommand, Order>(request);
            order.CreatedDate = _clock.UtcNow();

            await _orderRepository.CreateAsync(order).ConfigureAwait(false);

            var evt = _mapper.Map<Order, RunJoinedEvent>(order);

            await _eventPublisher.Publish(evt).ConfigureAwait(false);
        }
    }
}
