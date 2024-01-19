namespace TeaTime.Common.Features.Orders.Commands;

using Abstractions;

public record CreateOrderCommand(long Id, long RunId, long UserId, long OptionId) : BaseCommand, IUserCommand;
