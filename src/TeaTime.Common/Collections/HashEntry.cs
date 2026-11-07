namespace TeaTime.Common.Collections;

using System.Collections.Generic;

public record struct HashEntry(string Field, string Value)
{
    public static implicit operator HashEntry(KeyValuePair<string, string> keyValuePair) =>
        new(keyValuePair.Key, keyValuePair.Value);

    public static implicit operator KeyValuePair<string, string>(HashEntry hashEntry) =>
        new(hashEntry.Field, hashEntry.Value);
}
