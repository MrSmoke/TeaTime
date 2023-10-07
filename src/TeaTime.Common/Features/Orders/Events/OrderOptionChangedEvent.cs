namespace TeaTime.Common.Features.Orders.Events;

using Abstractions;

public record OrderOptionChangedEvent
(
    long OrderId,
    long UserId,
    long PreviousOptionId,
    long OptionId
) : Event;
