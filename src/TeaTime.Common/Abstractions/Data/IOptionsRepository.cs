namespace TeaTime.Common.Abstractions.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Data;

    public interface IOptionsRepository
    {
        Task CreateGroupAsync(RoomItemGroup group);
        Task UpdateGroupAsync(RoomItemGroup group);
        Task DeleteGroupAsync(long groupId);
        Task<RoomItemGroup?> GetGroupAsync(long groupId);
        Task<RoomItemGroup?> GetGroupByNameAsync(long roomId, string name);

        Task CreateAsync(Option option);
        Task<Option?> GetAsync(long optionId);
        Task UpdateAsync(Option option);
        Task DeleteAsync(long optionId);
        Task<IEnumerable<Option>> GetManyAsync(IEnumerable<long> ids);
        Task<IEnumerable<Option>> GetOptionsByGroupIdAsync(long groupId);
    }
}
