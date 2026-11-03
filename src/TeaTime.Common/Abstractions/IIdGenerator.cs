namespace TeaTime.Common.Abstractions;

using System.Threading;
using System.Threading.Tasks;

public interface IIdGenerator<T>
{
    Task<T> GenerateAsync(CancellationToken cancellationToken = default);
}
