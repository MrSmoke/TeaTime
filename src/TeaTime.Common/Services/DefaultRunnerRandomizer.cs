namespace TeaTime.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
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
            var userIds = orders.Select(o => o.User.Id).ToList();

            var random = _random.Next(userIds.Count - 1);

            return Task.FromResult(userIds[random]);
        }
    }
}
