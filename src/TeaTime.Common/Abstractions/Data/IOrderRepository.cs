namespace TeaTime.Common.Abstractions.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Data;

    public interface IOrderRepository
    {
        Task CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task<IEnumerable<Order>> GetOrdersAsync(long runId);
    }
}