namespace TeaTime.Common.Features.Options.Commands;

using Abstractions;

public record DeleteOptionCommand(long OptionId, long UserId) : BaseCommand, IUserCommand;
