namespace TeaTime.Common.Abstractions;

/// <summary>
/// A command which is issued by a user
/// </summary>
public interface IUserCommand : ICommand
{
    long UserId { get; }
}
