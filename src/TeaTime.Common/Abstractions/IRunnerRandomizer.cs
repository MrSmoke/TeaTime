namespace TeaTime.Common.Abstractions;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Domain;

public interface IRunnerRandomizer
{
    ValueTask<long> GetRunnerUserIdAsync(IEnumerable<OrderModel> orders, CancellationToken cancellationToken = default);
}
