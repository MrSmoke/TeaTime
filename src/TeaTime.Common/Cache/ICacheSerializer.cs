namespace TeaTime.Common.Cache
{
    public interface ICacheSerializer
    {
        byte[] Serialize(object value);
        T Deserialize<T>(byte[] bytes);
    }
}