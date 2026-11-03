namespace TeaTime.Common.Abstractions.Data;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IOrderRepository
{
    Task CreateAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetAsync(long id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Order>> GetOrdersAsync(long runId, CancellationToken cancellationToken = default);
}
