namespace TeaTime.Common.Abstractions
{
    using System.Threading.Tasks;

    public interface IIdGenerator<T>
    {
        ValueTask<T> GenerateAsync();
    }
}
