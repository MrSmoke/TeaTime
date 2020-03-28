namespace TeaTime.Common.Cache
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using MediatR;
    using Microsoft.Extensions.Caching.Distributed;

    public class CacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ICacheSerializer _cacheSerializer;


        public CacheBehaviour(IDistributedCache distributedCache, ICacheSerializer cacheSerializer)
        {
            _distributedCache = distributedCache;
            _cacheSerializer = cacheSerializer;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!(request is ICacheableQuery cacheQuery))
                return await next();

            var cacheKey = cacheQuery.CacheKey;

            // try and get from the cache first
            var bytes = await _distributedCache.GetAsync(cacheKey, cancellationToken);
            if (bytes != null)
                return _cacheSerializer.Deserialize<TResponse>(bytes);

            // no result found in cache, so run the rest of the pipeline and cache the result
            var response = await next();

            // dont cache null values
            if (response == null)
                return default;

            // serialize and store in the cache
            await StoreAsync(cacheKey, cacheQuery, response, cancellationToken);

            return response;
        }

        private async Task StoreAsync(string cacheKey, ICacheableQuery cacheQuery, TResponse response,
            CancellationToken cancellationToken)
        {
            var bytes = _cacheSerializer.Serialize(response);

            var cacheOptions = new DistributedCacheEntryOptions();

            if (cacheQuery.SlidingCache)
                cacheOptions.SetSlidingExpiration(cacheQuery.CacheExpiry);
            else
                cacheOptions.SetAbsoluteExpiration(cacheQuery.CacheExpiry);

            await _distributedCache.SetAsync(cacheKey, bytes, cacheOptions, cancellationToken);
        }
    }
}