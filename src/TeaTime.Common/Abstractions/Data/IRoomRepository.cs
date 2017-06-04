namespace TeaTime.Common.Abstractions.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IRoomRepository
    {
        Task<bool> Create(Room room);
        Task<Room> Get(Guid id);

        Task<bool> AddGroup(Group group);
        Task<Group> GetGroupByName(Guid roomId, string name);
        Task<bool> DeleteGroup(Guid groupId);

        Task<bool> AddGroupOption(Option option);
        Task<IEnumerable<Option>> GetGroupOptions(Guid groupId);
        Task<bool> DeleteGroupOption(Guid optionId);
    }
}