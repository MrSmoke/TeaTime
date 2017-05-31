namespace TeaTime.Common.Abstractions
{
    using System.Threading.Tasks;

    public interface ILinkable<T>
    {
        Task<T> GetByLink(string link);
        Task<bool> AddLink(string link, T obj);
    }
}