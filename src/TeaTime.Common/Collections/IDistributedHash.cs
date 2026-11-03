namespace TeaTime.Common.Collections;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IDistributedHash
{
    Task SetAsync(string key, string field, string value, CancellationToken cancellationToken = default);
    Task SetAsync(string key, IEnumerable<HashEntry> fields, CancellationToken cancellationToken = default);
    Task<IEnumerable<HashEntry>> GetAllAsync(string key, CancellationToken cancellationToken = default);
    Task<string?> GetValueAsync(string key, string field, CancellationToken cancellationToken = default);
}
