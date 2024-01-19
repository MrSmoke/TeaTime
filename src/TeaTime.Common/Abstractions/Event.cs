namespace TeaTime.Common.Abstractions;

using System;
using System.Collections.Generic;

public abstract record Event : IEvent
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public Dictionary<string, object> State { get; init; } = new();
}
