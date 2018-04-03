namespace TeaTime.Common.Abstractions.Data
{
    using System.Threading.Tasks;
    using Models.Data;

    public interface IUserRepository
    {
        Task Create(User user);
        Task<User> Get(long id);
    }
}