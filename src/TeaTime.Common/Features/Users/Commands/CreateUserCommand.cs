namespace TeaTime.Common.Features.Users.Commands
{
    using Abstractions;

    public class CreateUserCommand : ICommand
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
    }
}
