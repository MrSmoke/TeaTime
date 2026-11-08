namespace TeaTime.Common.Features.Orders.Commands;

using Abstractions;
using Models.Data;

public record UpdateOrderOptionCommand(OrderEntity Order, long CurrentUserId, long OptionId) : BaseCommand, IUserCommand;
