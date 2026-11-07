namespace TeaTime.Common.Abstractions;

using System.Threading;
using System.Threading.Tasks;

public interface ILinkable<T>
{
    Task<T> GetByLinkAsync(string link, CancellationToken cancellationToken = default);
    Task<bool> AddLinkAsync(string link, T obj, CancellationToken cancellationToken = default);
}
