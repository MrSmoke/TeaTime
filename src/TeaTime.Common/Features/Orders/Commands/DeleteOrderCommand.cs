namespace TeaTime.Common.Features.Orders.Commands;

using Abstractions;
using Models.Data;

public record DeleteOrderCommand(Order Order, long UserId) : BaseCommand, IUserCommand;
