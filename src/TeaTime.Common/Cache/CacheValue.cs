namespace TeaTime.Common.Cache
{
    public class CacheValue<T>
    {
        public CacheValue(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
