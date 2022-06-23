namespace TeaTime.Data.MySql.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class OrderRepository : BaseRepository, IOrderRepository
    {
        private const string SelectColumns = "id, runId, userId, optionId, createdDate";

        public OrderRepository(IMySqlConnectionFactory factory) : base(factory)
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
            //todo: update modified date (also add modified date)
            const string sql = "UPDATE orders SET optionId = @optionId WHERE id = @id";

            return ExecuteAsync(sql, order);
        }

        public Task<Order?> GetAsync(long id)
        {
            const string sql = "SELECT " + SelectColumns + " FROM orders where id = @id";

            return SingleOrDefaultAsync<Order>(sql, new {id});
        }

        public Task<IEnumerable<Order>> GetOrdersAsync(long runId)
        {
            const string sql = "SELECT " + SelectColumns + " FROM orders where runId = @runId";

            return QueryAsync<Order>(sql, new {runId});
        }
    }
}
