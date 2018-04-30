namespace TeaTime.Common.Abstractions.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Data;

    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User> GetAsync(long id);
        Task<IEnumerable<User>> GetManyAsync(IEnumerable<long> ids);
    }
}