namespace TeaTime.Common.Features.Orders.Commands
{
    using Abstractions;

    public class UpdateOrderOptionCommand : BaseCommand, IUserCommand
    {
        public long OrderId { get; }
        public long UserId { get; }
        public long OptionId { get; }

        public UpdateOrderOptionCommand(long orderId, long userId, long optionId)
        {
            OrderId = orderId;
            UserId = userId;
            OptionId = optionId;
        }
    }
}