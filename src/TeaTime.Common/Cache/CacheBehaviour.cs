namespace TeaTime.Common.Cache;

using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;

public class CacheBehaviour<TRequest, TResponse>(HybridCache cache) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheQuery)
            return await next(cancellationToken);

        var options = new HybridCacheEntryOptions
        {
            Expiration = cacheQuery.CacheExpiry,
            LocalCacheExpiration = cacheQuery.CacheExpiry
        };

        return await cache.GetOrCreateAsync(
            key: cacheQuery.CacheKey,
            state: next,
            factory: static async (handler, token) => await handler(token),
            options: options,
            cancellationToken: cancellationToken);
    }
}
