namespace TeaTime.Common.Abstractions.Data;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IOrderRepository
{
    Task CreateAsync(OrderEntity order, CancellationToken cancellationToken = default);
    Task UpdateAsync(OrderEntity order, CancellationToken cancellationToken = default);
    Task<OrderEntity?> GetAsync(long id, CancellationToken cancellationToken = default);

    Task<IEnumerable<OrderEntity>> GetOrdersAsync(long runId, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
