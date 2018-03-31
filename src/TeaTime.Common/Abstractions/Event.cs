namespace TeaTime.Common.Abstractions
{
    using System;

    public abstract class Event : IEvent
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
