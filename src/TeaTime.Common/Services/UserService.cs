namespace TeaTime.Common.Services
{
    using System;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using Models;

    public class UserService : IUserService
    {
        private readonly ILinkRepository _linkRepository;

        public UserService(ILinkRepository linkRepository)
        {
            _linkRepository = linkRepository;
        }

        public async Task<User> GetByLink(string link)
        {
            var userId = await _linkRepository.GetObjectId<Guid>(link, LinkType.User).ConfigureAwait(false);

            return await Get(userId).ConfigureAwait(false);
        }

        public Task<bool> AddLink(string link, User obj)
        {
            return _linkRepository.Add(obj.Id, LinkType.User, link);
        }

        public Task<User> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<User> Create(User user)
        {
            throw new NotImplementedException();
        }
    }
}
