namespace TeaTime.Common.Features.Orders.Events
{
    using Abstractions;

    public class OrderPlacedEvent : Event
    {
        public long OrderId { get; set; }
        public long RunId { get; set; }

        public long UserId { get; set; }
        public long OptionId { get; set; }
    }
}
