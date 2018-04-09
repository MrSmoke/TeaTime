namespace TeaTime.Common.Features.Orders.Commands
{
    using Abstractions;

    public class CreateOrderCommand : BaseCommand, IUserCommand
    {
        public long Id { get; }
        public long UserId { get; }
        public long OptionId { get; }
        public long RunId { get; }

        public CreateOrderCommand(long id, long runId, long userId, long optionId)
        {
            Id = id;
            RunId = runId;
            UserId = userId;
            OptionId = optionId;
        }
    }
}
