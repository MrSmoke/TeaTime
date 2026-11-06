namespace TeaTime.Common.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Exceptions;
using FakeItEasy;
using Features.Orders;
using Features.Orders.Commands;
using Features.Orders.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Data;
using Xunit;

public class OrderCommandHandlerTests
{
    private const long AvailableOptionId = 1;
    private const long UnavailableOptionId = 2;
    private const long DeletedOptionId = 3;

    private const long ActiveRunId = 7;
    private const long EndedRunId = 8;

    private static readonly Order ExistingOrder = new()
    {
        Id = 20,
        RunId = ActiveRunId,
        UserId = 5,
        OptionId = UnavailableOptionId,
        CreatedDate = DateTimeOffset.UnixEpoch
    };

    private readonly IOrderRepository orderRepository = CreateOrderRepository();
    private readonly IEventPublisher eventPublisher = CreateEventPublisher();
    private readonly OrderCommandHandler handler;

    public OrderCommandHandlerTests()
    {
        handler = new OrderCommandHandler(
            orderRepository: orderRepository,
            optionsRepository: CreateOptionsRepository(),
            runRepository: CreateRunRepository(),
            clock: TimeProvider.System,
            eventPublisher: eventPublisher,
            logger: NullLogger<OrderCommandHandler>.Instance
        );
    }

    // An available option is ordered as normal
    [Fact]
    public async Task Handle_Create_AvailableOption_SavesOrder()
    {
        var saved = A.Captured<Order>();
        A.CallTo(() => orderRepository.CreateAsync(saved._, A<CancellationToken>._)).Returns(Task.CompletedTask);

        var command = new CreateOrderCommand(Id: 30, RunId: ActiveRunId, UserId: 5, OptionId: AvailableOptionId);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        var order = saved.GetLastValue();
        Assert.Equal(30, order.Id);
        Assert.Equal(AvailableOptionId, order.OptionId);
    }

    // Unavailable and deleted options can't be ordered
    [Theory]
    [InlineData(UnavailableOptionId)]
    [InlineData(DeletedOptionId)]
    public async Task Handle_Create_OptionNotAvailable_Throws(long optionId)
    {
        var command = new CreateOrderCommand(Id: 30, RunId: ActiveRunId, UserId: 5, OptionId: optionId);

        var e = await Assert.ThrowsAsync<OrderException>(
            () => handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Equal(OrderException.OrderExceptionReason.OptionUnavailable, e.Reason);
        A.CallTo(() => orderRepository.CreateAsync(A<Order>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // Switching an existing order to an unavailable or deleted option is rejected
    [Theory]
    [InlineData(UnavailableOptionId)]
    [InlineData(DeletedOptionId)]
    public async Task Handle_Update_OptionNotAvailable_Throws(long optionId)
    {
        var order = ExistingOrder with { OptionId = AvailableOptionId };
        A.CallTo(() => orderRepository.GetAsync(order.Id, A<CancellationToken>._)).Returns(order);

        var command = new UpdateOrderOptionCommand(order, UserId: 5, OptionId: optionId);

        var e = await Assert.ThrowsAsync<OrderException>(
            () => handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Equal(OrderException.OrderExceptionReason.OptionUnavailable, e.Reason);
        A.CallTo(() => orderRepository.UpdateAsync(A<Order>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // Picking the option already ordered is a no-op, even once that option has become unavailable
    [Fact]
    public async Task Handle_Update_SameOption_DoesNothing()
    {
        var command = new UpdateOrderOptionCommand(ExistingOrder, UserId: 5, OptionId: UnavailableOptionId);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        A.CallTo(() => orderRepository.UpdateAsync(A<Order>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // Switching an order on an active round to an available option saves it and publishes the change
    [Fact]
    public async Task Handle_Update_AvailableOption_UpdatesOrder()
    {
        var saved = A.Captured<Order>();
        A.CallTo(() => orderRepository.UpdateAsync(saved._, A<CancellationToken>._)).Returns(Task.CompletedTask);

        var published = A.Captured<OrderOptionChangedEvent>();
        A.CallTo(() => eventPublisher.PublishAsync(published._, A<CancellationToken>._)).Returns(Task.CompletedTask);

        var command = new UpdateOrderOptionCommand(ExistingOrder, UserId: 5, OptionId: AvailableOptionId);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        var order = Assert.Single(saved.Values);
        Assert.Equal(AvailableOptionId, order.OptionId);

        var evt = Assert.Single(published.Values);
        Assert.Equal(UnavailableOptionId, evt.PreviousOptionId);
        Assert.Equal(AvailableOptionId, evt.OptionId);
    }

    // Leaving an active round deletes the order and publishes the removal
    [Fact]
    public async Task Handle_Delete_ActiveRun_DeletesOrder()
    {
        var published = A.Captured<OrderDeletedEvent>();
        A.CallTo(() => eventPublisher.PublishAsync(published._, A<CancellationToken>._)).Returns(Task.CompletedTask);

        var command = new DeleteOrderCommand(ExistingOrder, UserId: 5);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        var evt = Assert.Single(published.Values);
        Assert.Equal(ExistingOrder, evt.Order);
    }

    // Orders can't be placed on a round that has ended
    [Fact]
    public async Task Handle_Create_RunEnded_Throws()
    {
        var command = new CreateOrderCommand(Id: 30, RunId: EndedRunId, UserId: 5, OptionId: AvailableOptionId);

        var e = await Assert.ThrowsAsync<OrderException>(
            () => handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Equal(OrderException.OrderExceptionReason.RunEnded, e.Reason);
        A.CallTo(() => orderRepository.CreateAsync(A<Order>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // Orders on a round that has ended can't switch option
    [Fact]
    public async Task Handle_Update_RunEnded_Throws()
    {
        var order = ExistingOrder with { RunId = EndedRunId };
        A.CallTo(() => orderRepository.GetAsync(order.Id, A<CancellationToken>._)).Returns(order);

        var command = new UpdateOrderOptionCommand(order, UserId: 5, OptionId: AvailableOptionId);

        var e = await Assert.ThrowsAsync<OrderException>(
            () => handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Equal(OrderException.OrderExceptionReason.RunEnded, e.Reason);
        A.CallTo(() => orderRepository.UpdateAsync(A<Order>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // Orders on a round that has ended can't be removed
    [Fact]
    public async Task Handle_Delete_RunEnded_Throws()
    {
        var order = ExistingOrder with { RunId = EndedRunId };
        var command = new DeleteOrderCommand(order, UserId: 5);

        var e = await Assert.ThrowsAsync<OrderException>(
            () => handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Equal(OrderException.OrderExceptionReason.RunEnded, e.Reason);
        A.CallTo(() => orderRepository.DeleteAsync(A<long>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    private static IOrderRepository CreateOrderRepository()
    {
        var repository = A.Fake<IOrderRepository>(o => o.Strict());

        A.CallTo(() => repository.GetAsync(ExistingOrder.Id, A<CancellationToken>._))
            .Returns(ExistingOrder);

        A.CallTo(() => repository.CreateAsync(A<Order>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);

        A.CallTo(() => repository.UpdateAsync(A<Order>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);

        A.CallTo(() => repository.DeleteAsync(ExistingOrder.Id, A<CancellationToken>._))
            .Returns(true);

        return repository;
    }

    // Deleted options aren't returned by the repository
    private static IOptionsRepository CreateOptionsRepository()
    {
        var repository = A.Fake<IOptionsRepository>(o => o.Strict());

        A.CallTo(() => repository.GetAsync(AvailableOptionId, A<CancellationToken>._))
            .Returns(CreateOption(AvailableOptionId, available: true));

        A.CallTo(() => repository.GetAsync(UnavailableOptionId, A<CancellationToken>._))
            .Returns(CreateOption(UnavailableOptionId, available: false));

        A.CallTo(() => repository.GetAsync(DeletedOptionId, A<CancellationToken>._))
            .Returns((Option?)null);

        return repository;
    }

    private static IRunRepository CreateRunRepository()
    {
        var repository = A.Fake<IRunRepository>(o => o.Strict());

        A.CallTo(() => repository.GetAsync(ActiveRunId, A<CancellationToken>._))
            .Returns(CreateRun(ActiveRunId, ended: false));

        A.CallTo(() => repository.GetAsync(EndedRunId, A<CancellationToken>._))
            .Returns(CreateRun(EndedRunId, ended: true));

        return repository;
    }

    private static IEventPublisher CreateEventPublisher()
    {
        var publisher = A.Fake<IEventPublisher>(o => o.Strict());

        A.CallTo(() => publisher.PublishAsync(A<OrderPlacedEvent>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);

        return publisher;
    }

    private static Run CreateRun(long id, bool ended) => new()
    {
        Id = id,
        RoomId = 1,
        UserId = 5,
        GroupId = 1,
        StartTime = DateTimeOffset.UnixEpoch,
        Ended = ended,
        CreatedDate = DateTimeOffset.UnixEpoch
    };

    private static Option CreateOption(long id, bool available) => new()
    {
        Id = id,
        Name = "option" + id,
        Available = available,
        GroupId = 1,
        CreatedBy = 1,
        CreatedDate = DateTimeOffset.UnixEpoch
    };
}
