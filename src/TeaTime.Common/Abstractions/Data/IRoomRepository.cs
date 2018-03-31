namespace TeaTime.Common.Abstractions.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Data;

    public interface IRoomRepository
    {
        Task CreateAsync(Room room);
        Task<Room> GetAsync(long id);

        Task CreateCurrentRunAsync(long roomId, long runId);
        Task<long> GetCurrentRunAsync(long roomId);
        Task DeleteCurrentRunAsync(long roomId);

        //Task AddGroupAsync(RoomGroup group);
        //Task<RoomGroup> GetGroupByNameAsync(long roomId, string name);
        //Task DeleteGroupAsync(long groupId);

        //Task AddGroupOptionAsync(Option option);
        //Task<IEnumerable<Option>> GetGroupOptionsAsync(long groupId);
        //Task DeleteGroupOptionAsync(long optionId);
    }
}