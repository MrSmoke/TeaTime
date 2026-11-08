namespace TeaTime.Common.Features.Options.Commands;

using Abstractions;

public record SetOptionAvailabilityCommand(long OptionId, long CurrentUserId, bool Available) : BaseCommand, IUserCommand;
