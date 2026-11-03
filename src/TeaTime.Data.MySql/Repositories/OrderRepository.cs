namespace TeaTime.Data.MySql.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Abstractions.Data;
using Common.Models.Data;
using Factories;

public class OrderRepository(IMySqlConnectionFactory factory) : BaseRepository(factory), IOrderRepository
{
    private const string SelectColumns = "id, runId, userId, optionId, createdDate";

    public Task CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO orders " +
                           "(id, runId, userId, optionId, createdDate) VALUES " +
                           "(@id, @runId, @userId, @optionId, @createdDate)";

        return ExecuteAsync(sql, order, cancellationToken);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        //todo: update modified date (also add modified date)
        const string sql = "UPDATE orders SET optionId = @optionId WHERE id = @id";

        return ExecuteAsync(sql, order, cancellationToken);
    }

    public Task<Order?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT " + SelectColumns + " FROM orders where id = @id";

        return SingleOrDefaultAsync<Order>(sql, new { id }, cancellationToken);
    }

    public Task<IEnumerable<Order>> GetOrdersAsync(long runId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT " + SelectColumns + " FROM orders where runId = @runId";

        return QueryAsync<Order>(sql, new { runId }, cancellationToken);
    }
}
