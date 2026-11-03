namespace TeaTime.Common.Cache;

using System;

public class CacheEntryOptions
{
    /// <summary>
    /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
    /// This will not extend the entry lifetime beyond the absolute expiration (if set).
    /// </summary>
    public TimeSpan? Expiration
    {
        get;
        set
        {
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(Expiration),
                    value,
                    "The sliding expiration value must be positive.");
            }

            field = value;
        }
    } = TimeSpan.FromMinutes(5);

    public bool Sliding { get; set; } = true;
}
