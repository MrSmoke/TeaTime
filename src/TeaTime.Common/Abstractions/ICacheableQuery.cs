namespace TeaTime.Common.Abstractions
{
    using System;

    /// <summary>
    /// Defines a query which can be cached
    /// </summary>
    public interface ICacheableQuery
    {
        string CacheKey { get; }
        TimeSpan CacheExpiry => TimeSpan.FromMinutes(5);
        bool SlidingCache => false;
    }
}
