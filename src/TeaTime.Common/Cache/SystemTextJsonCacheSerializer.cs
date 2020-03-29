namespace TeaTime.Common.Cache
{
    using System;
    using System.Text.Json;

    public class SystemTextJsonCacheSerializer : ICacheSerializer
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions();

        public SystemTextJsonCacheSerializer() : this(DefaultOptions)
        {
        }

        public SystemTextJsonCacheSerializer(JsonSerializerOptions jsonSerializerOptions)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public byte[] Serialize(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return JsonSerializer.SerializeToUtf8Bytes(value, _jsonSerializerOptions);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            return JsonSerializer.Deserialize<T>(bytes, _jsonSerializerOptions);
        }
    }
}
