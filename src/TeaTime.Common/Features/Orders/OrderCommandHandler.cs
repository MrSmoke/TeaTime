namespace TeaTime.Common.Features.Orders;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Commands;
using Events;
using Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Models.Data;

public class OrderCommandHandler(
    IOrderRepository orderRepository,
    IOptionsRepository optionsRepository,
    IRunRepository runRepository,
    TimeProvider clock,
    IEventPublisher eventPublisher,
    ILogger<OrderCommandHandler> logger) :
    IRequestHandler<CreateOrderCommand>,
    IRequestHandler<UpdateOrderOptionCommand>,
    IRequestHandler<DeleteOrderCommand>
{
    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await EnsureRunActiveAsync(request.RunId, cancellationToken);
        await EnsureOptionAvailableAsync(request.OptionId, cancellationToken);

        var order = new OrderEntity
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

        await EnsureRunActiveAsync(existing.RunId, cancellationToken);

        // If the option is the same, skip
        if (existing.OptionId == request.OptionId)
            return;

        await EnsureOptionAvailableAsync(request.OptionId, cancellationToken);

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
        await EnsureRunActiveAsync(request.Order.RunId, cancellationToken);

        // Nothing was deleted, so don't publish an event
        if (!await orderRepository.DeleteAsync(request.Order.Id, cancellationToken))
            return;

        var evt = new OrderDeletedEvent(request.Order)
        {
            State = request.State
        };

        await eventPublisher.PublishAsync(evt, cancellationToken);
    }

    private async Task EnsureRunActiveAsync(long runId, CancellationToken cancellationToken)
    {
        var run = await runRepository.GetAsync(runId, cancellationToken);

        if (run is not { Ended: false })
        {
            throw new OrderException($"Run {runId} has ended or doesn't exist",
                OrderException.OrderExceptionReason.RunEnded);
        }
    }

    private async Task EnsureOptionAvailableAsync(long optionId, CancellationToken cancellationToken)
    {
        // Deleted options aren't returned, so they count as unavailable too
        var option = await optionsRepository.GetAsync(optionId, cancellationToken);

        if (option is not { Available: true })
        {
            throw new OrderException($"Option {optionId} is unavailable",
                OrderException.OrderExceptionReason.OptionUnavailable);
        }
    }
}
