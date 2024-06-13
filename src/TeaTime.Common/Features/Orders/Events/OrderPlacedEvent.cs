namespace TeaTime.Common.Features.Orders.Events;

using Abstractions;

public record OrderPlacedEvent
(
    long OrderId,
    long RunId,
    long UserId,
    long OptionId
) : BaseEvent;
