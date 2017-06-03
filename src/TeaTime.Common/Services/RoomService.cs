namespace TeaTime.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using Models;

    public class RoomService : IRoomService
    {
        private readonly ILinkRepository _linkRepository;

        public RoomService(ILinkRepository linkRepository)
        {
            _linkRepository = linkRepository;
        }

        public async Task<Room> GetByLink(string link)
        {
            var objectId = await _linkRepository.GetObjectId<Guid>(link, LinkType.Room).ConfigureAwait(false);

            if (objectId.Equals(Guid.Empty))
                return null;

            return await Get(objectId).ConfigureAwait(false);
        }

        public Task<bool> AddLink(string link, Room obj)
        {
            return _linkRepository.Add(obj.Id, LinkType.Room, link);
        }

        public Task<Room> Create(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Room> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Option> AddOption(Room room, string @group, string option)
        {
            throw new NotImplementedException();
        }

        public Task RemoveOption(Room room, string @group, Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Option>> GetOptions(Room room, string @group)
        {
            throw new NotImplementedException();
        }
    }
}
