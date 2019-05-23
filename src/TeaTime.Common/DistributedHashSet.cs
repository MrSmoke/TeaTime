namespace TeaTime.Common
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDistributedHash
    {
        Task SetAsync(string key, string field, string value);
        Task SetAsync(string key, IEnumerable<HashEntry> fields);
        Task<IEnumerable<HashEntry>> GetAllAsync(string key);
        Task<string> GetValueAsync(string key, string field);
    }

    public struct HashEntry
    {
        public string Field { get; set; }
        public string Value { get; set; }

        public HashEntry(string field, string value)
        {
            Field = field;
            Value = value;
        }

        public static implicit operator HashEntry(KeyValuePair<string, string> keyValuePair) =>
            new HashEntry(keyValuePair.Key, keyValuePair.Value);

        public static implicit operator KeyValuePair<string, string>(HashEntry hashEntry) =>
            new KeyValuePair<string, string>(hashEntry.Field, hashEntry.Value);
    }
}
