namespace TeaTime.Common.Features.IllMake.Commands
{
    using Abstractions;

    public class IllMakeCommand : BaseCommand, IUserCommand
    {
        public long Id { get; }
        public long RunId { get; }
        public long UserId { get; }

        public IllMakeCommand(long id, long runId, long userId)
        {
            Id = id;
            RunId = runId;
            UserId = userId;
        }
    }
}
