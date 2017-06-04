namespace TeaTime.Common.Services
{
    using System;
    using System.Threading.Tasks;
    using Abstractions;
    using Models;

    public interface IUserService : ILinkable<User>
    {
        Task<User> Get(Guid id);
        Task<User> Create(string name);
    }
}
