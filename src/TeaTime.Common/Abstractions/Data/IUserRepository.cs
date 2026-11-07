namespace TeaTime.Common.Abstractions.Data;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IUserRepository
{
    Task CreateAsync(UserEntity user, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserEntity>> GetManyAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
}
