namespace TeaTime.Common.Features.Users.Commands
{
    using Abstractions;

    public class CreateUserCommand : ICommand
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }

        public CreateUserCommand(long id, string username, string displayName)
        {
            Id = id;
            Username = username;
            DisplayName = displayName;
        }
    }
}
