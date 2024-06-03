namespace TeaTime.Common.Cache
{
    using System;

    public class CacheEntryOptions
    {
        private TimeSpan? _expiration = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
        /// This will not extend the entry lifetime beyond the absolute expiration (if set).
        /// </summary>
        public TimeSpan? Expiration
        {
            get => _expiration;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(Expiration),
                        value,
                        "The sliding expiration value must be positive.");
                }

                _expiration = value;
            }
        }

        public bool Sliding { get; set; } = true;
    }
}
