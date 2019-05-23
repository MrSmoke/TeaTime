namespace TeaTime.Common.Abstractions
{
    using System.Threading.Tasks;

    public interface IIdGenerator<T>
    {
        Task<T> GenerateAsync();
    }
}