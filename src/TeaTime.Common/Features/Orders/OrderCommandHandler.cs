namespace TeaTime.Common.Features.Orders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using Commands;
    using Events;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Models.Data;

    public class OrderCommandHandler :
        IRequestHandler<CreateOrderCommand>,
        IRequestHandler<UpdateOrderOptionCommand>,
        IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly TimeProvider _clock;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<OrderCommandHandler> _logger;

        public OrderCommandHandler(IOrderRepository orderRepository,
            TimeProvider clock,
            IEventPublisher eventPublisher,
            ILogger<OrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _clock = clock;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                Id = request.Id,
                CreatedDate = _clock.GetUtcNow(),
                OptionId = request.OptionId,
                RunId = request.RunId,
                UserId = request.UserId
            };

            await _orderRepository.CreateAsync(order);

            var evt = new OrderPlacedEvent
            (
                OrderId: order.Id,
                RunId: order.RunId,
                UserId: order.UserId,
                OptionId: order.OptionId
            )
            {
                State = request.State
            };

            await _eventPublisher.PublishAsync(evt);
        }

        public async Task Handle(UpdateOrderOptionCommand request, CancellationToken cancellationToken)
        {
            var existing = await _orderRepository.GetAsync(request.OrderId);

            if (existing is null)
            {
                _logger.LogWarning("Failed to get existing order {OrderId}", request.OrderId);
                return;
            }

            if (existing.OptionId == request.OptionId)
                return;

            //store the previous id for the event
            var previousOptionId = existing.OptionId;

            //update
            await _orderRepository.UpdateAsync(existing with { OptionId = request.OptionId });

            //create event
            var evt = new OrderOptionChangedEvent
            (
                OrderId: request.OrderId,
                UserId: request.UserId,
                PreviousOptionId: previousOptionId,
                OptionId: request.OptionId
            )
            {
                State = request.State,
            };

            await _eventPublisher.PublishAsync(evt);
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            // If we return false, then nothing was deleted, so dont publish an event
            if (!await _orderRepository.DeleteAsync(request.Order.Id))
                return;

            await _eventPublisher.PublishAsync(new OrderDeletedEvent(request.Order));
        }
    }
}
