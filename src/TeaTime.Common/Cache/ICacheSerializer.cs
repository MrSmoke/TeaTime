namespace TeaTime.Common.Cache
{
    using System;

    public interface ICacheSerializer
    {
        byte[] Serialize(object value);
        T? Deserialize<T>(ReadOnlySpan<byte> bytes);
    }
}
