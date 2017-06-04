namespace TeaTime.Data.MySql.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models;
    using Dapper;

    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(ConnectionFactory factory) : base(factory)
        {
        }

        public async Task<bool> Create(User user)
        {
            const string sql = "INSERT INTO users (id, name, date_created) VALUES (@id, @name, @dateCreated)";

            var rows = await GetConnection(conn => conn.ExecuteAsync(sql, user)).ConfigureAwait(false);

            return rows == 1;
        }

        public Task<User> Get(Guid id)
        {
            const string sql = "SELECT id, name, date_created as DateCreated FROM users WHERE id = @id LIMIT 1";

            return GetConnection(conn => conn.QuerySingleOrDefaultAsync<User>(sql, new { id }));
        }
    }
}