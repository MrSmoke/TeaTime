namespace TeaTime.Common.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models.Data;
    using Models.Domain;
    using Services;
    using Xunit;

    public class DefaultRunnerRandomizerTests
    {
        /// <summary>
        /// Test to see if the DefaultRunnerRandomizer will eventually select all users from the given list over 200 iterations
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetRunnerUserId_SelectsAll()
        {
            var randomizer = new DefaultRunnerRandomizer();

            var orders = new List<OrderModel>
            {
                new OrderModel
                {
                    User = new User
                    {
                        Id = 5
                    }
                },
                new OrderModel
                {
                    User = new User
                    {
                        Id = 6
                    }
                },
                new OrderModel
                {
                    User = new User
                    {
                        Id = 7
                    }
                },
                new OrderModel
                {
                    User = new User
                    {
                        Id = 8
                    }
                }
            };

            var results = new List<long>();

            for (var i = 0; i < 200; i++)
            {
                results.Add(await randomizer.GetRunnerUserId(orders));
            }

            Assert.Equal(orders.Count, results.Distinct().Count());
        }
    }
}
