namespace TeaTime.Common.Cache;

using System.Threading;
using System.Threading.Tasks;

public interface ICache
{
    Task<CacheValue<T>?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync(string key, object value, CacheEntryOptions? options, CancellationToken cancellationToken = default);
}
