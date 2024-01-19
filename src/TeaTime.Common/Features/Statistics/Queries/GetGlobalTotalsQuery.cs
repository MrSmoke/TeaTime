namespace TeaTime.Common.Features.Statistics.Queries
{
    using System;
    using Abstractions;
    using Models;

    public record GetGlobalTotalsQuery : IQuery<GlobalTotals>, ICacheableQuery
    {
        public string CacheKey => "global-totals";
        public bool SlidingCache => false;
        public TimeSpan CacheExpiry => TimeSpan.FromHours(1);
    }
}
