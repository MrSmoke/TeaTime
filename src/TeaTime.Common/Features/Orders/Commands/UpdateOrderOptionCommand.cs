namespace TeaTime.Common.Features.Orders.Commands;

using Abstractions;

public record UpdateOrderOptionCommand(long OrderId, long UserId, long OptionId) : BaseCommand, IUserCommand;
