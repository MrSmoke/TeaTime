namespace TeaTime.Common.Features.Runs.Events
{
    using Abstractions;

    public class RunJoinedEvent : Event
    {
        public long UserId { get; set; }
        public long RunId { get; set; }
    }
}
