namespace TeaTime.Common.Abstractions.Data;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IIllMakeRepository
{
    Task CreateAsync(IllMakeEntity obj, CancellationToken cancellationToken = default);
    Task<IEnumerable<IllMakeEntity>> GetAllByRunAsync(long runId, CancellationToken cancellationToken = default);
}
