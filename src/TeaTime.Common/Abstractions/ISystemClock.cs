namespace TeaTime.Common.Abstractions
{
    using System;

    public interface ISystemClock
    {
        DateTimeOffset UtcNow();
    }
}
