namespace TeaTime.Common.Cache;

using System;
using System.Text.Json;

public class SystemTextJsonCacheSerializer(JsonSerializerOptions jsonSerializerOptions) : ICacheSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new();

    public SystemTextJsonCacheSerializer() : this(DefaultOptions)
    {
    }

    public byte[] Serialize(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return JsonSerializer.SerializeToUtf8Bytes(value, jsonSerializerOptions);
    }

    public T? Deserialize<T>(ReadOnlySpan<byte> bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes, jsonSerializerOptions);
    }
}
