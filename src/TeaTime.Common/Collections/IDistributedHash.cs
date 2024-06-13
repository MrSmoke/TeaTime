namespace TeaTime.Common.Collections
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDistributedHash
    {
        Task SetAsync(string key, string field, string value);
        Task SetAsync(string key, IEnumerable<HashEntry> fields);
        Task<IEnumerable<HashEntry>> GetAllAsync(string key);
        Task<string?> GetValueAsync(string key, string field);
    }
}
