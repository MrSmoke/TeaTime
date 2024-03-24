namespace TeaTime.Common.Cache
{
    using System;
    using System.Text.Json;

    public class SystemTextJsonCacheSerializer : ICacheSerializer
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        private static readonly JsonSerializerOptions DefaultOptions = new();

        public SystemTextJsonCacheSerializer() : this(DefaultOptions)
        {
        }

        public SystemTextJsonCacheSerializer(JsonSerializerOptions jsonSerializerOptions)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public byte[] Serialize(object value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return JsonSerializer.SerializeToUtf8Bytes(value, _jsonSerializerOptions);
        }

        public T? Deserialize<T>(ReadOnlySpan<byte> bytes)
        {
            return JsonSerializer.Deserialize<T>(bytes, _jsonSerializerOptions);
        }
    }
}
