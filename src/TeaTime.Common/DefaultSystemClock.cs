namespace TeaTime.Common
{
    using System;
    using Abstractions;

    public class DefaultSystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}
