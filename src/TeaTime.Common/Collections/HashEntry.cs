namespace TeaTime.Common.Collections
{
    using System.Collections.Generic;

    public struct HashEntry(string field, string value)
    {
        public string Field { get; set; } = field;
        public string Value { get; set; } = value;

        public static implicit operator HashEntry(KeyValuePair<string, string> keyValuePair) =>
            new(keyValuePair.Key, keyValuePair.Value);

        public static implicit operator KeyValuePair<string, string>(HashEntry hashEntry) =>
            new(hashEntry.Field, hashEntry.Value);
    }
}
