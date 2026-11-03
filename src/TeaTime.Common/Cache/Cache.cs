namespace TeaTime.Common.Cache;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

public class Cache(IDistributedCache distributedCache, ICacheSerializer serializer) : ICache
{
    public async Task<CacheValue<T>?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var bytes = await distributedCache.GetAsync(key, cancellationToken);
        if (bytes == null)
            return null;

        var value = serializer.Deserialize<T>(bytes);
        if (value is null)
            return null;

        return new CacheValue<T>(value);
    }

    public Task SetAsync(string key, object value, CacheEntryOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        options ??= new CacheEntryOptions();

        var bytes = serializer.Serialize(value);

        var distributedCacheEntryOptions = new DistributedCacheEntryOptions();

        if (options.Expiration.HasValue)
        {
            if (options.Sliding)
                distributedCacheEntryOptions.SetSlidingExpiration(options.Expiration.Value);
            else
                distributedCacheEntryOptions.SetAbsoluteExpiration(options.Expiration.Value);
        }

        return distributedCache.SetAsync(key, bytes, distributedCacheEntryOptions, cancellationToken);
    }
}
