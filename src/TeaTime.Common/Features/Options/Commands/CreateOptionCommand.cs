namespace TeaTime.Common.Features.Options.Commands
{
    using Abstractions;

    public class CreateOptionCommand : IUserCommand
    {
        public long Id { get; }
        public long UserId { get; }
        public string Name { get; }

        public CreateOptionCommand(long id, long userId, string name)
        {
            Id = id;
            UserId = userId;
            Name = name;
        }
    }
}
