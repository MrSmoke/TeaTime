namespace TeaTime.Common.Features.Options.Commands;

using Abstractions;

public record CreateOptionCommand(long Id, long CurrentUserId, long GroupId, string Name) : IUserCommand;
