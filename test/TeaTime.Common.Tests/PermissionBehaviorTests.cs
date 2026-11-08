namespace TeaTime.Common.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Exceptions;
using Features.Orders.Commands;
using Features.Orders.Permissions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Data;
using Permissions;
using Xunit;

public class PermissionBehaviorTests
{
    // Requests that aren't user commands must skip the constrained behaviour instead of failing to resolve
    [Fact]
    public async Task Send_NonUserCommand_SkipsPermissionBehavior()
    {
        await using var provider = CreateProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new PingQuery(), TestContext.Current.CancellationToken);

        Assert.Equal("pong", result);
    }

    // The check registered for the concrete command type runs and rejects other users' orders
    [Fact]
    public async Task Send_DeleteOtherUsersOrder_ThrowsPermissionException()
    {
        await using var provider = CreateProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var handler = provider.GetRequiredService<HandledRecorder>();

        var command = new DeleteOrderCommand(CreateOrder(userId: 1), CurrentUserId: 2);

        var ex = await Assert.ThrowsAsync<PermissionException>(
            () => mediator.Send(command, TestContext.Current.CancellationToken));

        Assert.Equal("You cannot delete other users' orders", ex.Message);
        Assert.False(handler.Handled);
    }

    // A user deleting their own order passes the check and reaches the handler
    [Fact]
    public async Task Send_DeleteOwnOrder_ReachesHandler()
    {
        await using var provider = CreateProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var handler = provider.GetRequiredService<HandledRecorder>();

        var command = new DeleteOrderCommand(CreateOrder(userId: 1), CurrentUserId: 1);

        await mediator.Send(command, TestContext.Current.CancellationToken);

        Assert.True(handler.Handled);
    }

    // The update check rejects changing another user's order before the handler runs
    [Fact]
    public async Task Send_UpdateOtherUsersOrder_ThrowsPermissionException()
    {
        await using var provider = CreateProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var handler = provider.GetRequiredService<HandledRecorder>();

        var command = new UpdateOrderOptionCommand(CreateOrder(userId: 1), CurrentUserId: 2, OptionId: 40);

        var ex = await Assert.ThrowsAsync<PermissionException>(
            () => mediator.Send(command, TestContext.Current.CancellationToken));

        Assert.Equal("You cannot change other users' orders", ex.Message);
        Assert.False(handler.Handled);
    }

    // A user changing their own order passes the check and reaches the handler
    [Fact]
    public async Task Send_UpdateOwnOrder_ReachesHandler()
    {
        await using var provider = CreateProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var handler = provider.GetRequiredService<HandledRecorder>();

        var command = new UpdateOrderOptionCommand(CreateOrder(userId: 1), CurrentUserId: 1, OptionId: 40);

        await mediator.Send(command, TestContext.Current.CancellationToken);

        Assert.True(handler.Handled);
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSingleton<IPermissionService, PermissionService>();
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(PermissionBehavior<,>));
        services.AddSingleton<IPermissionCheck<DeleteOrderCommand>, OrderPermissions>();
        services.AddSingleton<IPermissionCheck<UpdateOrderOptionCommand>, OrderPermissions>();

        services.AddSingleton<HandledRecorder>();
        services.AddMediatR(o => o.RegisterServicesFromAssemblyContaining<PermissionBehaviorTests>());

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
    }

    private static OrderEntity CreateOrder(long userId) => new()
    {
        Id = 10,
        CreatedDate = DateTimeOffset.UnixEpoch,
        RunId = 20,
        UserId = userId,
        OptionId = 30
    };

    public record PingQuery : IRequest<string>;

    public class PingQueryHandler : IRequestHandler<PingQuery, string>
    {
        public Task<string> Handle(PingQuery request, CancellationToken cancellationToken) =>
            Task.FromResult("pong");
    }

    public class HandledRecorder
    {
        public bool Handled { get; set; }
    }

    public class OrderHandler(HandledRecorder recorder) :
        IRequestHandler<DeleteOrderCommand>,
        IRequestHandler<UpdateOrderOptionCommand>
    {
        public Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            recorder.Handled = true;
            return Task.CompletedTask;
        }

        public Task Handle(UpdateOrderOptionCommand request, CancellationToken cancellationToken)
        {
            recorder.Handled = true;
            return Task.CompletedTask;
        }
    }
}
