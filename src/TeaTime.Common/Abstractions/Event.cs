namespace TeaTime.Common.Abstractions
{
    using System;
    using System.Collections.Generic;

    public abstract record Event : IEvent
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
        public Dictionary<string, object> State { get; set; } = new();
    }
}
