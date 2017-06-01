namespace TeaTime.Common.Services
{
    using System;
    using System.Threading.Tasks;
    using Models;

    public class RoomService : IRoomService
    {
        private readonly ILinkService _linkService;

        public RoomService(ILinkService linkService)
        {
            _linkService = linkService;
        }

        public async Task<Room> GetByLink(string link)
        {
            var objectId = await _linkService.GetLinkedObjectId<Guid>(link, LinkType.Room).ConfigureAwait(false);

            if (objectId.Equals(Guid.Empty))
                return null;

            return await Get(objectId).ConfigureAwait(false);
        }

        public Task<bool> AddLink(string link, Room obj)
        {
            return _linkService.AddLink(obj.Id, LinkType.Room, link);
        }

        public Task<Room> Create()
        {
            throw new NotImplementedException();
        }

        public Task<Room> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task AddOption(Room room, string option)
        {
            throw new NotImplementedException();
        }

        public Task RemoveOption(Room room, string option)
        {
            throw new NotImplementedException();
        }
    }
}
