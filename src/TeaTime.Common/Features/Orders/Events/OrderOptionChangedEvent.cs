namespace TeaTime.Common.Features.Orders.Events
{
    using Abstractions;

    public class OrderOptionChangedEvent : Event
    {
        public long OrderId { get; set; }
        public long UserId { get; set; }

        public long PreviousOptionId { get; set; }
        public long OptionId { get; set; }
    }
}