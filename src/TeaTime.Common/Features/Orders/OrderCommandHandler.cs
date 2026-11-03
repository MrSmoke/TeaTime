namespace TeaTime.Common.Features.Orders;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Commands;
using Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Models.Data;

public class OrderCommandHandler(
    IOrderRepository orderRepository,
    TimeProvider clock,
    IEventPublisher eventPublisher,
    ILogger<OrderCommandHandler> logger) :
    IRequestHandler<CreateOrderCommand>,
    IRequestHandler<UpdateOrderOptionCommand>,
    IRequestHandler<DeleteOrderCommand>
{
    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = request.Id,
            CreatedDate = clock.GetUtcNow(),
            OptionId = request.OptionId,
            RunId = request.RunId,
            UserId = request.UserId
        };

        await orderRepository.CreateAsync(order, cancellationToken);

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

        await eventPublisher.PublishAsync(evt, cancellationToken);
    }

    public async Task Handle(UpdateOrderOptionCommand request, CancellationToken cancellationToken)
    {
        // Re-read so a concurrent leave or option change isn't overwritten
        var existing = await orderRepository.GetAsync(request.Order.Id, cancellationToken);

        if (existing is null)
        {
            logger.LogWarning("Failed to get existing order {OrderId}", request.Order.Id);
            return;
        }

        if (existing.OptionId == request.OptionId)
            return;

        //store the previous id for the event
        var previousOptionId = existing.OptionId;

        //update
        await orderRepository.UpdateAsync(existing with { OptionId = request.OptionId }, cancellationToken);

        //create event
        var evt = new OrderOptionChangedEvent
        (
            OrderId: request.Order.Id,
            UserId: request.UserId,
            PreviousOptionId: previousOptionId,
            OptionId: request.OptionId
        )
        {
            State = request.State,
        };

        await eventPublisher.PublishAsync(evt, cancellationToken);
    }

    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        // Nothing was deleted, so don't publish an event
        if (!await orderRepository.DeleteAsync(request.Order.Id, cancellationToken))
            return;

        var evt = new OrderDeletedEvent(request.Order)
        {
            State = request.State
        };

        await eventPublisher.PublishAsync(evt, cancellationToken);
    }
}
