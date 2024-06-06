namespace TeaTime.Common.Features.Orders.Events;

using Abstractions;
using Models.Data;

public record OrderDeletedEvent(Order Order) : BaseEvent;
