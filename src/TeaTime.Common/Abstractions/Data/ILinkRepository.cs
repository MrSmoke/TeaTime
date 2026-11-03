namespace TeaTime.Common.Abstractions.Data;

using System.Threading;
using System.Threading.Tasks;
using Models;

public interface ILinkRepository
{
    Task<long?> GetObjectIdAsync(string link, LinkType linkType, CancellationToken cancellationToken = default);
    Task AddAsync(long objectId, LinkType linkType, string link, CancellationToken cancellationToken = default);

    Task<string?> GetLinkAsync(long objectId, LinkType linkType, CancellationToken cancellationToken = default);
}
