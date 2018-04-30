namespace TeaTime.Common.Features.Options.Commands
{
    using Abstractions;

    public class DeleteOptionCommand : BaseCommand, IUserCommand
    {
        public long OptionId { get; }
        public long UserId { get; }

        public DeleteOptionCommand(long optionId, long userId)
        {
            OptionId = optionId;
            UserId = userId;
        }
    }
}
