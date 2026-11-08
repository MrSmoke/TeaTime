namespace TeaTime.Common.Features.Orders.Commands;

using Abstractions;

public record CreateOrderCommand(long Id, long RunId, long CurrentUserId, long OptionId) : BaseCommand, IUserCommand;
