namespace TeaTime.Common.Abstractions.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using Models.Data;

    public interface IRoomRepository
    {
        Task Create(Room room);
        Task<Room> Get(Guid id);

        Task AddGroup(RoomGroup group);
        Task<RoomGroup> GetGroupByName(Guid roomId, string name);
        Task DeleteGroup(Guid groupId);

        Task AddGroupOption(Option option);
        Task<IEnumerable<Option>> GetGroupOptions(Guid groupId);
        Task DeleteGroupOption(Guid optionId);
    }
}