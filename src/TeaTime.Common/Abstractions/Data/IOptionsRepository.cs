namespace TeaTime.Common.Abstractions.Data;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IOptionsRepository
{
    Task CreateGroupAsync(RoomItemGroup group, CancellationToken cancellationToken = default);
    Task UpdateGroupAsync(RoomItemGroup group, CancellationToken cancellationToken = default);
    Task DeleteGroupAsync(long groupId, CancellationToken cancellationToken = default);
    Task<RoomItemGroup?> GetGroupAsync(long groupId, CancellationToken cancellationToken = default);
    Task<RoomItemGroup?> GetGroupByNameAsync(long roomId, string name, CancellationToken cancellationToken = default);

    Task CreateAsync(Option option, CancellationToken cancellationToken = default);
    Task<Option?> GetAsync(long optionId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Option option, CancellationToken cancellationToken = default);
    Task SetAvailabilityAsync(long optionId, bool available, CancellationToken cancellationToken = default);
    Task DeleteAsync(long optionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Option>> GetManyAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<Option>> GetOptionsByGroupIdAsync(long groupId, CancellationToken cancellationToken = default);
}
