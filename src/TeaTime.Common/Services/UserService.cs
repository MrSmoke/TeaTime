namespace TeaTime.Common.Services
{
    using System;
    using System.Threading.Tasks;
    using Models;

    public class UserService : IUserService
    {
        private readonly ILinkService _linkService;

        public UserService(ILinkService linkService)
        {
            _linkService = linkService;
        }

        public async Task<User> GetByLink(string link)
        {
            var userId = await _linkService.GetLinkedObjectId<Guid>(link, LinkType.User).ConfigureAwait(false);

            return await Get(userId).ConfigureAwait(false);
        }

        public Task<bool> AddLink(string link, User obj)
        {
            return _linkService.AddLink(obj.Id, LinkType.User, link);
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
