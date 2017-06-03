namespace TeaTime.Data.MySql.Repositories
{
    using Common.Abstractions.Data;

    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(ConnectionFactory factory) : base(factory)
        {
        }
    }
}