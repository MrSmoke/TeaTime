namespace TeaTime.Common.Collections
{
    using System.Collections.Generic;

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
            new(keyValuePair.Key, keyValuePair.Value);

        public static implicit operator KeyValuePair<string, string>(HashEntry hashEntry) =>
            new(hashEntry.Field, hashEntry.Value);
    }
}
