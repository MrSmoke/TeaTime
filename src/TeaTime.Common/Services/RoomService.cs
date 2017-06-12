namespace TeaTime.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using Exceptions;
    using Models;

    public class RoomService : IRoomService
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IRoomRepository _roomRepository;

        public RoomService(ILinkRepository linkRepository, IRoomRepository roomRepository)
        {
            _linkRepository = linkRepository;
            _roomRepository = roomRepository;
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

        public async Task<Room> Create(string name)
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = name,
                DateCreated = DateTime.UtcNow
            };

            if(!await _roomRepository.Create(room).ConfigureAwait(false))
                throw new TeaTimeException("Failed to create room");

            return room;
        }

        public Task<Room> Get(Guid id)
        {
            return _roomRepository.Get(id);
        }

        public async Task<Group> AddGroup(Room room, string name)
        {
            var group = new Group
            {
                Name = name,
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                RoomId = room.Id
            };

            if(!await _roomRepository.AddGroup(group).ConfigureAwait(false))
                throw new TeaTimeException("Failed to create group");

            return group;
        }

        public Task<Group> GetGroupByName(Room room, string name)
        {
            return _roomRepository.GetGroupByName(room.Id, name);
        }

        public async Task<Option> AddOption(Group group, string text)
        {
            var option = new Option
            {
                Name = text,
                Id = Guid.NewGuid(),
                RoomGroupId = group.Id,
                DateCreated = DateTime.UtcNow
            };

            if(!await _roomRepository.AddGroupOption(option).ConfigureAwait(false))
                throw new TeaTimeException("Failed to create option");

            return option;
        }

        public Task RemoveOption(Guid optionId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveOption(Group group, Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Option>> GetOptions(Group group)
        {
            return _roomRepository.GetGroupOptions(group.Id);
        }
    }
}
