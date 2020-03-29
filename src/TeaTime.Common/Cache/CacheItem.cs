namespace TeaTime.Common.Cache
{
    public class CacheItem<T>
    {
        public CacheItem(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
