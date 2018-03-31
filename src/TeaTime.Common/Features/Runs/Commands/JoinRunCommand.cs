namespace TeaTime.Common.Features.Runs.Commands
{
    using Abstractions;

    public class JoinRunCommand : IUserCommand
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long RunId { get; set; }
    }
}
