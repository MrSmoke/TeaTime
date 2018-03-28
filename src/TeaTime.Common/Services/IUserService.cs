namespace TeaTime.Common.Services
{
    using System;
    using System.Threading.Tasks;
    using Abstractions;
    using Models;
    using Models.Data;

    public interface IUserService : ILinkable<User>
    {
        Task<User> Get(Guid id);
        Task<User> Create(string name);
    }
}
