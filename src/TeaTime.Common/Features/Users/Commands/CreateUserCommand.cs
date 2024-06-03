namespace TeaTime.Common.Features.Users.Commands;

using Abstractions;

public record CreateUserCommand(long Id, string Username, string DisplayName) : ICommand;
