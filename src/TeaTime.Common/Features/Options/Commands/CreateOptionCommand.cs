namespace TeaTime.Common.Features.Options.Commands;

using Abstractions;

public record CreateOptionCommand(long Id, long UserId, long GroupId, string Name) : IUserCommand;
