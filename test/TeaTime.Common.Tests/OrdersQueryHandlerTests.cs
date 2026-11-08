namespace TeaTime.Common.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Exceptions;
using FakeItEasy;
using Features.Orders;
using Features.Orders.Queries;
using Models.Data;
using Xunit;

public class OrdersQueryHandlerTests
{
    private static readonly RunEntity Run = new()
    {
        Id = 1,
        RoomId = 1,
        UserId = 1,
        GroupId = 1,
        StartTime = DateTimeOffset.UnixEpoch,
        CreatedDate = DateTimeOffset.UnixEpoch
    };

    private readonly IOrderRepository _orderRepository = A.Fake<IOrderRepository>();
    private readonly IUserRepository _userRepository = A.Fake<IUserRepository>();
    private readonly IRunRepository _runRepository = A.Fake<IRunRepository>();

    public OrdersQueryHandlerTests()
    {
        A.CallTo(() => _runRepository.GetAsync(Run.Id, A<CancellationToken>._)).Returns(Run);
    }

    // Orders come back sorted by the ordering user's display name, not by the order the repository returns them in
    [Fact]
    public async Task Handle_GetRunOrders_SortsByUserDisplayName()
    {
        string[] names = ["Charlie", "Alice", "Bob"];
        var users = names.Select((name, i) => CreateUser(id: i + 1, name)).ToList();
        var orders = users.Select(u => CreateOrder(id: u.Id * 10, userId: u.Id)).ToList();

        SetupOrders(orders, users);

        var query = new GetRunOrdersQuery(RunId: Run.Id, UserId: 1);
        var result = await CreateHandler().Handle(query, TestContext.Current.CancellationToken);

        var displayNames = result.Select(o => o.User?.DisplayName);
        Assert.Equal(["Alice", "Bob", "Charlie"], displayNames);
    }

    // An order whose user no longer exists has a null user and sorts before named users
    [Fact]
    public async Task Handle_GetRunOrders_MissingUserSortsFirst()
    {
        var users = new List<UserEntity> { CreateUser(id: 1, "Alice") };
        List<OrderEntity> orders = [CreateOrder(id: 10, userId: 1), CreateOrder(id: 20, userId: 99)];

        SetupOrders(orders, users);

        var query = new GetRunOrdersQuery(RunId: Run.Id, UserId: 1);
        var result = (await CreateHandler().Handle(query, TestContext.Current.CancellationToken)).ToList();

        Assert.Equal([20L, 10L], result.Select(o => o.Id));
        Assert.Null(result[0].User);
    }

    [Fact]
    public async Task Handle_GetRunOrders_UnknownRun_Throws()
    {
        A.CallTo(() => _runRepository.GetAsync(A<long>._, A<CancellationToken>._)).Returns((RunEntity?)null);

        var query = new GetRunOrdersQuery(RunId: 404, UserId: 1);

        await Assert.ThrowsAsync<TeaTimeException>(
            () => CreateHandler().Handle(query, TestContext.Current.CancellationToken));
    }

    private static UserEntity CreateUser(long id, string displayName) => new()
    {
        Id = id,
        DisplayName = displayName,
        Username = displayName.ToLowerInvariant(),
        CreatedDate = DateTimeOffset.UnixEpoch
    };

    private static OrderEntity CreateOrder(long id, long userId) => new()
    {
        Id = id,
        RunId = Run.Id,
        UserId = userId,
        OptionId = 1,
        CreatedDate = DateTimeOffset.UnixEpoch
    };

    private void SetupOrders(List<OrderEntity> orders, List<UserEntity> users)
    {
        A.CallTo(() => _orderRepository.GetOrdersAsync(Run.Id, A<CancellationToken>._)).Returns(orders);
        A.CallTo(() => _userRepository.GetManyAsync(A<IEnumerable<long>>._, A<CancellationToken>._)).Returns(users);
    }

    private OrdersQueryHandler CreateHandler() => new(
        orderRepository: _orderRepository,
        userRepository: _userRepository,
        optionsRepository: A.Fake<IOptionsRepository>()
    );
}
