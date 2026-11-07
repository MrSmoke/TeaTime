namespace TeaTime.Common.Features.Orders.Commands;

using Abstractions;
using Models.Data;

public record DeleteOrderCommand(OrderEntity Order, long UserId) : BaseCommand, IUserCommand;
