namespace TeaTime.Data.MySql.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Abstractions.Data;
using Common.Models.Data;
using Factories;

public class UserRepository(IMySqlConnectionFactory factory) : BaseRepository(factory), IUserRepository
{
    public Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO users (id, username, displayName, createdDate) VALUES (@id, @username, @displayName, @createdDate)";

        return ExecuteAsync(sql, user, cancellationToken);
    }

    public Task<User?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT id, username, displayName, createdDate FROM users WHERE id = @id";

        return SingleOrDefaultAsync<User>(sql, new { id }, cancellationToken);
    }

    public Task<IEnumerable<User>> GetManyAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT id, username, displayName, createdDate FROM users WHERE id IN @ids";

        return QueryAsync<User>(sql, new { ids }, cancellationToken);
    }
}
