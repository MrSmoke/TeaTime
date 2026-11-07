namespace TeaTime.Data.MySql.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Abstractions.Data;
using Common.Models.Data;
using Factories;

public class IllMakeRepository(IMySqlConnectionFactory factory) : BaseRepository(factory), IIllMakeRepository
{
    public Task CreateAsync(IllMakeEntity obj, CancellationToken cancellationToken = default)
    {
        const string sql =
            "INSERT INTO illmakes (id, runId, userId, createdDate) VALUES (@id, @runId, @userId, @createdDate)";

        return ExecuteAsync(sql, obj, cancellationToken);
    }

    public Task<IEnumerable<IllMakeEntity>> GetAllByRunAsync(long runId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM illmakes WHERE runId = @runId";

        return QueryAsync<IllMakeEntity>(sql, new { runId }, cancellationToken);
    }
}
