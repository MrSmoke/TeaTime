namespace TeaTime.Data.MySql.Repositories;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Abstractions.Data;
using Common.Models.Data;
using Factories;

public class OrderRepository(IMySqlConnectionFactory factory, TimeProvider systemClock)
    : BaseRepository(factory), IOrderRepository
{
    private const string SelectColumns = "id, runId, userId, optionId, createdDate";

    public Task CreateAsync(OrderEntity order, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           INSERT INTO orders
                           (id, runId, userId, optionId, createdDate) VALUES
                           (@id, @runId, @userId, @optionId, @createdDate)
                           """;

        return ExecuteAsync(sql, order, cancellationToken);
    }

    public Task UpdateAsync(OrderEntity order, CancellationToken cancellationToken = default)
    {
        //todo: update modified date (also add modified date)
        const string sql = "UPDATE orders SET optionId = @optionId WHERE id = @id AND deleted = 0";

        return ExecuteAsync(sql, order, cancellationToken);
    }

    public Task<OrderEntity?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT " + SelectColumns + " FROM orders WHERE id = @id AND deleted = 0";

        return SingleOrDefaultAsync<OrderEntity>(sql, new { id }, cancellationToken);
    }

    public Task<IEnumerable<OrderEntity>> GetOrdersAsync(long runId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT " + SelectColumns + " FROM orders WHERE runId = @runId AND deleted = 0";

        return QueryAsync<OrderEntity>(sql, new { runId }, cancellationToken);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           UPDATE orders
                           SET deleted = 1, deletedDate = @now
                           WHERE id = @id AND deleted = 0
                           """;

        var rows = await ExecuteAsync(sql, new
        {
            id,
            now = systemClock.GetUtcNow()
        }, cancellationToken);

        return rows > 0;
    }
}
