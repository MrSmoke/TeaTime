namespace TeaTime.Common.Features.IllMake.Commands;

using Abstractions;

public record IllMakeCommand(long Id, long RunId, long CurrentUserId) : BaseCommand, IUserCommand;
