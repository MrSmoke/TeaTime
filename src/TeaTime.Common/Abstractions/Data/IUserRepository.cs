namespace TeaTime.Common.Abstractions.Data
{
    using System;
    using System.Threading.Tasks;
    using Models;

    public interface IUserRepository
    {
        Task<bool> Create(User user);
        Task<User> Get(Guid id);
    }
}