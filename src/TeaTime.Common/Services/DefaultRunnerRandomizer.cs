namespace TeaTime.Common.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Abstractions;
    using Extensions;
    using Models.Domain;

    public class DefaultRunnerRandomizer : IRunnerRandomizer
    {
        public Task<long> GetRunnerUserId(IEnumerable<OrderModel> orders)
        {
            var userIds = orders
                .WhereNotNull(o => o.User)
                .Select(o => o.Id)
                .ToList();

            var random = RandomNumberGenerator.GetInt32(userIds.Count);;

            return Task.FromResult(userIds[random]);
        }
    }
}
