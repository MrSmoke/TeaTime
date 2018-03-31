namespace TeaTime.Data.MySql.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(ConnectionFactory factory) : base(factory)
        {
        }

        public Task CreateAsync(Order order)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(Order order)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetOrdersAsync(long runId)
        {
            throw new System.NotImplementedException();
        }
    }
}
