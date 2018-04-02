namespace TeaTime.Common.Features.Runs.Commands
{
    using Abstractions;

    public class JoinRunCommand : IUserCommand
    {
        public long Id { get; }
        public long UserId { get; }
        public long OptionId { get; }
        public long RunId { get; }

        public JoinRunCommand(long id, long runId, long userId, long optionId)
        {
            Id = id;
            RunId = runId;
            UserId = userId;
            OptionId = optionId;
        }
    }
}
