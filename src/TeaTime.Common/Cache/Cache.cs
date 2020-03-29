namespace TeaTime.Common.Cache
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Distributed;

    public class Cache : ICache
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ICacheSerializer _serializer;

        public Cache(IDistributedCache distributedCache, ICacheSerializer serializer)
        {
            _distributedCache = distributedCache;
            _serializer = serializer;
        }

        public async Task<CacheValue<T>> GetAsync<T>(string key, CancellationToken token = default)
        {
            var bytes = await _distributedCache.GetAsync(key, token);
            if (bytes == null)
                return null;

            var value = _serializer.Deserialize<T>(bytes);
            return new CacheValue<T>(value);
        }

        public Task SetAsync(string key, object value, CacheEntryOptions options, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            options ??= new CacheEntryOptions();

            var bytes = _serializer.Serialize(value);

            var distributedCacheEntryOptions = new DistributedCacheEntryOptions();

            if (options.Expiration.HasValue)
            {
                if (options.Sliding)
                    distributedCacheEntryOptions.SetSlidingExpiration(options.Expiration.Value);
                else
                    distributedCacheEntryOptions.SetAbsoluteExpiration(options.Expiration.Value);
            }

            return _distributedCache.SetAsync(key, bytes, distributedCacheEntryOptions, token);
        }
    }
}
