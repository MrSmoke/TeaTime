namespace TeaTime.Data.MySql.Repositories
{
    using System.Threading.Tasks;
    using Common.Abstractions.Data;
    using Common.Models.Data;

    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(ConnectionFactory factory) : base(factory)
        {
        }

        public Task Create(User user)
        {
            const string sql = "INSERT INTO users (id, username, displayName, createdDate) VALUES (@id, @username, @displayName, @createdDate)";

            return ExecuteAsync(sql, user);
        }

        public Task<User> Get(long id)
        {
            const string sql = "SELECT id, username, displayName, createdDate FROM users WHERE id = @id";

            return SingleOrDefaultAsync<User>(sql, new {id});
        }
    }
}