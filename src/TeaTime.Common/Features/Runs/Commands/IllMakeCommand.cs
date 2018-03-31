namespace TeaTime.Common.Features.Runs.Commands
{
    using Abstractions;

    public class IllMakeCommand : IUserCommand
    {
        public long UserId { get; set; }
        public long RunId { get; set; }
    }
}