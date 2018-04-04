namespace TeaTime.Data.MySql.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class OrderRepository : BaseRepository, IOrderRepository
    {
        private const string SelectColumns = "id, runId, userId, optionId, createdDate";

        public OrderRepository(ConnectionFactory factory) : base(factory)
        {
        }

        public Task CreateAsync(Order order)
        {
            const string sql = "INSERT INTO orders " +
                               "(id, runId, userId, optionId, createdDate) VALUES " +
                               "(@id, @runId, @userId, @optionId, @createdDate)";

            return ExecuteAsync(sql, order);
        }

        public Task UpdateAsync(Order order)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetOrdersAsync(long runId)
        {
            const string sql = "SELECT " + SelectColumns + " FROM orders where runId = @runId";

            return QueryAsync<Order>(sql, new {runId});
        }
    }
}
