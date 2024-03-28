namespace TeaTime.Common.Tests
{
    using System;
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
                new()
                {
                    User = new User
                    {
                        Id = 5,
                        Username = "test",
                        CreatedDate = DateTimeOffset.UtcNow,
                        DisplayName = "display test"
                    },
                    Id = 1,
                    Option = null!,
                    Run = null!,
                    CreatedDate = DateTimeOffset.UtcNow
                },
                new()
                {
                    User = new User
                    {
                        Id = 6,
                        Username = "test",
                        CreatedDate = DateTimeOffset.UtcNow,
                        DisplayName = "display test"
                    },
                    Id = 2,
                    Option = null!,
                    Run = null!,
                    CreatedDate = DateTimeOffset.UtcNow
                },
                new()
                {
                    User = new User
                    {
                        Id = 7,
                        Username = "test",
                        CreatedDate = DateTimeOffset.UtcNow,
                        DisplayName = "display test"
                    },
                    Id = 3,
                    Option = null!,
                    Run = null!,
                    CreatedDate = DateTimeOffset.UtcNow
                },
                new()
                {
                    User = new User
                    {
                        Id = 8,
                        Username = "test",
                        CreatedDate = DateTimeOffset.UtcNow,
                        DisplayName = "display test"
                    },
                    Id = 4,
                    Option = null!,
                    Run = null!,
                    CreatedDate = DateTimeOffset.UtcNow
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
