namespace TeaTime.Common.Features.Options.Commands;

using Abstractions;

public record SetOptionAvailabilityCommand(long OptionId, long UserId, bool Available) : BaseCommand, IUserCommand;
