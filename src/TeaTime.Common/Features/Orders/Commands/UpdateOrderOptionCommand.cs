namespace TeaTime.Common.Features.Orders.Commands;

using Abstractions;
using Models.Data;

public record UpdateOrderOptionCommand(OrderEntity Order, long UserId, long OptionId) : BaseCommand, IUserCommand;
