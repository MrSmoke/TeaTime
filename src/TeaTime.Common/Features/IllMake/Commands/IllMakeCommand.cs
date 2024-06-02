namespace TeaTime.Common.Features.IllMake.Commands;

using Abstractions;

public record IllMakeCommand(long Id, long RunId, long UserId) : BaseCommand, IUserCommand;
