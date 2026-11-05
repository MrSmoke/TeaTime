namespace TeaTime.Common.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using FakeItEasy;
using Features.Orders;
using Features.Orders.Queries;
using Models.Data;
using Xunit;

public class OrdersQueryHandlerTests
{
    // Orders come back sorted by the ordering user's display name, not by the order the repository returns them in
    [Fact]
    public async Task Handle_GetRunOrders_SortsByUserDisplayName()
    {
        string[] names = ["Charlie", "Alice", "Bob"];
        var users = names.Select((name, i) => CreateUser(id: i + 1, name)).ToList();
        var orders = users.Select(u => CreateOrder(id: u.Id * 10, userId: u.Id)).ToList();

        var run = new Run
        {
            Id = 1,
            RoomId = 1,
            UserId = 1,
            GroupId = 1,
            StartTime = DateTimeOffset.UnixEpoch,
            CreatedDate = DateTimeOffset.UnixEpoch
        };

        var orderRepository = A.Fake<IOrderRepository>();
        var userRepository = A.Fake<IUserRepository>();
        var runRepository = A.Fake<IRunRepository>();

        A.CallTo(() => runRepository.GetAsync(run.Id, A<CancellationToken>._)).Returns(run);
        A.CallTo(() => orderRepository.GetOrdersAsync(run.Id, A<CancellationToken>._)).Returns(orders);
        A.CallTo(() => userRepository.GetManyAsync(A<IEnumerable<long>>._, A<CancellationToken>._)).Returns(users);

        var handler = new OrdersQueryHandler(
            orderRepository: orderRepository,
            userRepository: userRepository,
            optionsRepository: A.Fake<IOptionsRepository>(),
            runRepository: runRepository
        );

        var query = new GetRunOrdersQuery(RunId: run.Id, UserId: 1);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        var displayNames = result.Select(o => o.User?.DisplayName);
        Assert.Equal(["Alice", "Bob", "Charlie"], displayNames);
    }

    private static User CreateUser(long id, string displayName) => new()
    {
        Id = id,
        DisplayName = displayName,
        Username = displayName.ToLowerInvariant(),
        CreatedDate = DateTimeOffset.UnixEpoch
    };

    private static Order CreateOrder(long id, long userId) => new()
    {
        Id = id,
        RunId = 1,
        UserId = userId,
        OptionId = 1,
        CreatedDate = DateTimeOffset.UnixEpoch
    };
}
