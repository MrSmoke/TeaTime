namespace TeaTime.Common.Services
{
    using System;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using Models;
    using Models.Data;

    public class UserService : IUserService
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IUserRepository _userRepository;

        public UserService(ILinkRepository linkRepository, IUserRepository userRepository)
        {
            _linkRepository = linkRepository;
            _userRepository = userRepository;
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
            return _userRepository.Get(id);
        }

        public async Task<User> Create(string name)
        {
            var user = new User
            {
                Name = name,

                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow
            };

            await _userRepository.Create(user).ConfigureAwait(false);

            return user;
        }
    }
}
