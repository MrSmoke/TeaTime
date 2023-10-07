namespace TeaTime.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using Extensions;
    using Models.Domain;

    public class DefaultRunnerRandomizer : IRunnerRandomizer
    {
        private readonly Random _random;

        public DefaultRunnerRandomizer()
        {
            _random = new Random();
        }

        public Task<long> GetRunnerUserId(IEnumerable<OrderModel> orders)
        {
            var userIds = orders
                .WhereNotNull(o => o.User)
                .Select(o => o.Id)
                .ToList();

            var random = _random.Next(userIds.Count);

            return Task.FromResult(userIds[random]);
        }
    }
}
