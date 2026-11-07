namespace TeaTime.Common.Abstractions.Data;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IOptionsRepository
{
    Task CreateGroupAsync(RoomItemGroupEntity group, CancellationToken cancellationToken = default);
    Task UpdateGroupAsync(RoomItemGroupEntity group, CancellationToken cancellationToken = default);
    Task DeleteGroupAsync(long groupId, CancellationToken cancellationToken = default);
    Task<RoomItemGroupEntity?> GetGroupAsync(long groupId, CancellationToken cancellationToken = default);
    Task<RoomItemGroupEntity?> GetGroupByNameAsync(long roomId, string name, CancellationToken cancellationToken = default);

    Task CreateAsync(OptionEntity option, CancellationToken cancellationToken = default);
    Task<OptionEntity?> GetAsync(long optionId, CancellationToken cancellationToken = default);
    Task UpdateAsync(OptionEntity option, CancellationToken cancellationToken = default);
    Task SetAvailabilityAsync(long optionId, bool available, CancellationToken cancellationToken = default);
    Task DeleteAsync(long optionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OptionEntity>> GetManyAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<OptionEntity>> GetOptionsByGroupIdAsync(long groupId, CancellationToken cancellationToken = default);
}
