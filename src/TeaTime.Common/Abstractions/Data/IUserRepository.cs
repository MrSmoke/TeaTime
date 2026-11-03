namespace TeaTime.Common.Abstractions.Data;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Data;

public interface IUserRepository
{
    Task CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetManyAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
}
