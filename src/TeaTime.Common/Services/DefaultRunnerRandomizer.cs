namespace TeaTime.Common.Services;

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Extensions;
using Models.Domain;

public class DefaultRunnerRandomizer : IRunnerRandomizer
{
    public ValueTask<long> GetRunnerUserIdAsync(IEnumerable<OrderModel> orders, CancellationToken cancellationToken = default)
    {
        var userIds = orders
            .WhereNotNull(o => o.User)
            .Select(o => o.Id)
            .ToList();

        var random = RandomNumberGenerator.GetInt32(userIds.Count);

        return new ValueTask<long>(userIds[random]);
    }
}
