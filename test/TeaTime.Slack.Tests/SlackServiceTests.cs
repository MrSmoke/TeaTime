namespace TeaTime.Slack.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Common.Abstractions;
using Common.Collections;
using Common.Exceptions;
using Common.Features.Links.Queries;
using Common.Features.Options.Queries;
using Common.Features.Orders.Commands;
using Common.Features.Orders.Queries;
using Common.Features.RoomItemGroups.Models;
using Common.Features.RoomItemGroups.Queries;
using Common.Features.Rooms.Queries;
using Common.Features.Runs.Queries;
using Common.Features.Users.Commands;
using Common.Features.Users.Queries;
using Common.Models.Data;
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Interactivity;
using Models.SlashCommands;
using Resources;
using Services;
using Xunit;

public class SlackServiceTests
{
    private const string ResponseUrl = "https://hooks.slack.com/actions/T1/1/abc";

    private const long RunGroupId = 100;

    // A join button for option 1 in run 7
    private const string JoinValue = "7:1";

    [Fact]
    public async Task HandleBlockActions_JoinFails_RepliesEphemerallyViaResponseUrl()
    {
        var replies = A.Captured<SlashCommandResponse>();
        var service = CreateService(CreateApiClient(replies));

        // No join action, so the join fails before touching the domain
        await service.HandleBlockActionsAsync(CreatePayload(ResponseUrl, actionId: "something_else"),
            TestContext.Current.CancellationToken);

        var reply = Assert.Single(replies.Values);
        Assert.Equal(ErrorStrings.General(), reply.Text);
        Assert.Equal(ResponseType.User, reply.Type);
    }

    [Fact]
    public async Task HandleBlockActions_NoResponseUrl_Ignored()
    {
        var apiClient = A.Fake<ISlackApiClient>(o => o.Strict());
        var service = CreateService(apiClient);

        await service.HandleBlockActionsAsync(CreatePayload(null, actionId: "join_option_1"),
            TestContext.Current.CancellationToken);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public async Task GetOrCreateUser_NoName_UsesFormattedFallback(string? name)
    {
        var mediator = A.Fake<ISender>();
        A.CallTo(() => mediator.Send(A<GetObjectIdByLinkValueQuery>._, A<CancellationToken>._))
            .Returns((long?)null);

        var idGenerator = A.Fake<IIdGenerator<long>>();
        A.CallTo(() => idGenerator.GenerateAsync(A<CancellationToken>._)).Returns(10L);

        var service = CreateService(A.Fake<ISlackApiClient>(), mediator, idGenerator);

        var user = await service.GetOrCreateUserAsync("U123", name, TestContext.Current.CancellationToken);

        Assert.Equal("Slack User U123", user.DisplayName);
        A.CallTo(() => mediator.Send(A<CreateUserCommand>.That.Matches(c => c.DisplayName == "Slack User U123"),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetOrCreateRoom_NoName_UsesFormattedFallback(string? name)
    {
        var mediator = A.Fake<ISender>();
        A.CallTo(() => mediator.Send(A<GetObjectIdByLinkValueQuery>._, A<CancellationToken>._))
            .Returns((long?)null);

        var idGenerator = A.Fake<IIdGenerator<long>>();
        A.CallTo(() => idGenerator.GenerateAsync(A<CancellationToken>._)).Returns(20L);

        var service = CreateService(A.Fake<ISlackApiClient>(), mediator, idGenerator);

        var room = await service.GetOrCreateRoomAsync("C123", name, userId: 1,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal("Slack Channel C123", room.Name);
    }

    [Fact]
    public async Task HandleBlockActions_OptionFromAnotherGroup_RepliesUnknownAndDoesNotOrder()
    {
        var replies = A.Captured<SlashCommandResponse>();
        var mediator = CreateJoinMediator(optionGroupId: 200);
        var service = CreateService(CreateApiClient(replies), mediator);

        await service.HandleBlockActionsAsync(CreatePayload(ResponseUrl, actionId: "join_option_1"),
            TestContext.Current.CancellationToken);

        var reply = Assert.Single(replies.Values);
        Assert.Equal(ErrorStrings.OptionUnknown(), reply.Text);

        // Sent via a BaseCommand variable, so match on that type
        A.CallTo(() => mediator.Send(A<BaseCommand>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // Clicking a button for an option that has since been marked unavailable replies with its name
    [Fact]
    public async Task HandleBlockActions_OptionUnavailable_RepliesUnavailable()
    {
        var idGenerator = A.Fake<IIdGenerator<long>>();
        A.CallTo(() => idGenerator.GenerateAsync(A<CancellationToken>._)).Returns(30L);

        var unavailable = new OrderException("Option 1 is unavailable",
            OrderException.OrderExceptionReason.OptionUnavailable);

        // Sent via a BaseCommand variable, so match on that type
        var mediator = CreateJoinMediator(optionGroupId: RunGroupId);
        A.CallTo(() => mediator.Send(A<BaseCommand>._, A<CancellationToken>._)).Throws(unavailable);

        var replies = A.Captured<SlashCommandResponse>();
        var service = CreateService(CreateApiClient(replies), mediator, idGenerator);
        var payload = CreatePayload(ResponseUrl, actionId: "join_option_1");

        await service.HandleBlockActionsAsync(payload, TestContext.Current.CancellationToken);

        var reply = Assert.Single(replies.Values);
        Assert.Equal(ErrorStrings.JoinRun_OptionUnavailable("Earl Grey"), reply.Text);
        Assert.Equal(ResponseType.User, reply.Type);
    }

    [Fact]
    public async Task HandleBlockActions_OptionFromRunGroup_CreatesOrder()
    {
        var replies = A.Captured<SlashCommandResponse>();
        var mediator = CreateJoinMediator(optionGroupId: RunGroupId);
        var idGenerator = A.Fake<IIdGenerator<long>>();
        A.CallTo(() => idGenerator.GenerateAsync(A<CancellationToken>._)).Returns(30L);
        var service = CreateService(CreateApiClient(replies), mediator, idGenerator);

        // Sent via a BaseCommand variable, so capture on that type
        var sent = A.Captured<BaseCommand>();
        A.CallTo(() => mediator.Send(sent._, A<CancellationToken>._)).Returns(Task.CompletedTask);

        await service.HandleBlockActionsAsync(CreatePayload(ResponseUrl, actionId: "join_option_1"),
            TestContext.Current.CancellationToken);

        var command = Assert.IsType<CreateOrderCommand>(Assert.Single(sent.Values));
        Assert.Equal(1, command.OptionId);
        Assert.Empty(replies.Values);
    }

    // Clicking a button when already in the round changes the existing order instead of creating another
    [Fact]
    public async Task HandleBlockActions_AlreadyOrdered_UpdatesExistingOrder()
    {
        var replies = A.Captured<SlashCommandResponse>();
        var mediator = CreateJoinMediator(optionGroupId: RunGroupId);

        var existingOrder = new OrderEntity
        {
            Id = 40,
            RunId = 7,
            UserId = 5,
            OptionId = 2,
            CreatedDate = DateTimeOffset.UtcNow
        };

        A.CallTo(() => mediator.Send(A<GetUserOrderQuery>._, A<CancellationToken>._))
            .Returns(existingOrder);

        // Strict id generator by default, so creating a new order would fail the test
        var service = CreateService(CreateApiClient(replies), mediator);

        // Sent via a BaseCommand variable, so capture on that type
        var sent = A.Captured<BaseCommand>();
        A.CallTo(() => mediator.Send(sent._, A<CancellationToken>._)).Returns(Task.CompletedTask);

        await service.HandleBlockActionsAsync(CreatePayload(ResponseUrl, actionId: "join_option_1"),
            TestContext.Current.CancellationToken);

        var command = Assert.IsType<UpdateOrderOptionCommand>(Assert.Single(sent.Values));
        Assert.Equal(existingOrder, command.Order);
        Assert.Equal(5, command.CurrentUserId);
        Assert.Equal(1, command.OptionId);
        Assert.Empty(replies.Values);
    }

    // A round ending after the button's run check still replies ended when the order is rejected
    [Fact]
    public async Task HandleBlockActions_JoinRejectedRunEnded_RepliesRunEnded()
    {
        var idGenerator = A.Fake<IIdGenerator<long>>();
        A.CallTo(() => idGenerator.GenerateAsync(A<CancellationToken>._)).Returns(30L);

        var ended = new OrderException("Run 7 has ended", OrderException.OrderExceptionReason.RunEnded);
        var mediator = CreateJoinMediator(optionGroupId: RunGroupId);

        // Sent via a BaseCommand variable, so match on that type
        A.CallTo(() => mediator.Send(A<BaseCommand>._, A<CancellationToken>._)).Throws(ended);

        var replies = A.Captured<SlashCommandResponse>();
        var service = CreateService(CreateApiClient(replies), mediator, idGenerator);

        await service.HandleBlockActionsAsync(CreatePayload(ResponseUrl, actionId: "join_option_1"),
            TestContext.Current.CancellationToken);

        var reply = Assert.Single(replies.Values);
        Assert.Equal(ErrorStrings.JoinRun_RunEnded(), reply.Text);

        // The button's run is used, not the room's current run
        A.CallTo(() => mediator.Send(A<GetCurrentRunQuery>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // A button carrying a run from another room doesn't order into it
    [Fact]
    public async Task HandleBlockActions_RunFromAnotherRoom_RepliesNotStartedAndDoesNotOrder()
    {
        var replies = A.Captured<SlashCommandResponse>();
        var mediator = CreateJoinMediator(optionGroupId: RunGroupId);

        A.CallTo(() => mediator.Send(A<GetRunQuery>._, A<CancellationToken>._))
            .Returns(CreateRun(roomId: 6));

        var service = CreateService(CreateApiClient(replies), mediator);

        await service.HandleBlockActionsAsync(CreatePayload(ResponseUrl, actionId: "join_option_1"),
            TestContext.Current.CancellationToken);

        var reply = Assert.Single(replies.Values);
        Assert.Equal(ErrorStrings.JoinRun_RunNotStarted(), reply.Text);

        // Sent via a BaseCommand variable, so match on that type
        A.CallTo(() => mediator.Send(A<BaseCommand>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // Joining by name with the slash command matches the option name ignoring case
    [Fact]
    public async Task JoinRun_OptionNameDifferentCase_CreatesOrder()
    {
        var mediator = CreateJoinMediator(optionGroupId: RunGroupId);

        var option = new OptionEntity
        {
            Id = 1,
            Name = "Earl Grey",
            Available = true,
            GroupId = RunGroupId,
            CreatedBy = 5,
            CreatedDate = DateTimeOffset.UtcNow
        };

        var group = new RoomItemGroupModel
        {
            Id = RunGroupId,
            Name = "drinks",
            RoomId = 5,
            CreatedBy = 5,
            CreatedDate = DateTimeOffset.UtcNow,
            Options = [option]
        };

        A.CallTo(() => mediator.Send(A<GetRoomItemGroupQuery>._, A<CancellationToken>._))
            .Returns(group);

        var idGenerator = A.Fake<IIdGenerator<long>>();
        A.CallTo(() => idGenerator.GenerateAsync(A<CancellationToken>._)).Returns(30L);

        var service = CreateService(A.Fake<ISlackApiClient>(), mediator, idGenerator);

        var slashCommand = new SlashCommand
        {
            Command = "/teatime",
            Text = "join earl grey",
            ResponseUrl = ResponseUrl,
            UserId = "U1",
            UserName = "steve",
            ChannelId = "C1",
            ChannelName = "general"
        };

        // Sent via a BaseCommand variable, so capture on that type
        var sent = A.Captured<BaseCommand>();
        A.CallTo(() => mediator.Send(sent._, A<CancellationToken>._)).Returns(Task.CompletedTask);

        await service.JoinRunAsync(slashCommand, "earl grey", TestContext.Current.CancellationToken);

        var command = Assert.IsType<CreateOrderCommand>(Assert.Single(sent.Values));
        Assert.Equal(1, command.OptionId);
    }

    // A join button on an ended round's message says the round ended, even if the option is no longer in its group
    [Fact]
    public async Task HandleBlockActions_JoinEndedRun_RepliesRunEnded()
    {
        var replies = A.Captured<SlashCommandResponse>();
        var mediator = CreateJoinMediator(optionGroupId: 200);

        A.CallTo(() => mediator.Send(A<GetRunQuery>._, A<CancellationToken>._))
            .Returns(CreateRun(roomId: 5, ended: true));

        var service = CreateService(CreateApiClient(replies), mediator);

        await service.HandleBlockActionsAsync(CreatePayload(ResponseUrl, actionId: "join_option_1"),
            TestContext.Current.CancellationToken);

        var reply = Assert.Single(replies.Values);
        Assert.Equal(ErrorStrings.JoinRun_RunEnded(), reply.Text);

        // Sent via a BaseCommand variable, so match on that type
        A.CallTo(() => mediator.Send(A<BaseCommand>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // A leave button on an ended round's message says the round ended, even if the user never joined it
    [Fact]
    public async Task HandleBlockActions_LeaveEndedRun_RepliesRunEnded()
    {
        var replies = A.Captured<SlashCommandResponse>();
        var mediator = CreateJoinMediator(optionGroupId: RunGroupId);

        A.CallTo(() => mediator.Send(A<GetRunQuery>._, A<CancellationToken>._))
            .Returns(CreateRun(roomId: 5, ended: true));

        var service = CreateService(CreateApiClient(replies), mediator);
        var payload = CreatePayload(ResponseUrl, actionId: BlockBuilder.LeaveRunActionId, value: "7");

        await service.HandleBlockActionsAsync(payload, TestContext.Current.CancellationToken);

        var reply = Assert.Single(replies.Values);
        Assert.Equal(ErrorStrings.LeaveRun_RunEnded(), reply.Text);

        // Sent via a BaseCommand variable, so match on that type
        A.CallTo(() => mediator.Send(A<BaseCommand>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // A round ending after the leave button's run check still replies ended when the delete is rejected
    [Fact]
    public async Task HandleBlockActions_LeaveRejectedRunEnded_RepliesRunEnded()
    {
        var replies = A.Captured<SlashCommandResponse>();
        var mediator = CreateJoinMediator(optionGroupId: RunGroupId);

        var existingOrder = new OrderEntity
        {
            Id = 40,
            RunId = 7,
            UserId = 5,
            OptionId = 1,
            CreatedDate = DateTimeOffset.UtcNow
        };

        A.CallTo(() => mediator.Send(A<GetUserOrderQuery>._, A<CancellationToken>._))
            .Returns(existingOrder);

        var ended = new OrderException("Run 7 has ended", OrderException.OrderExceptionReason.RunEnded);
        A.CallTo(() => mediator.Send(A<DeleteOrderCommand>._, A<CancellationToken>._)).Throws(ended);

        var service = CreateService(CreateApiClient(replies), mediator);
        var payload = CreatePayload(ResponseUrl, actionId: BlockBuilder.LeaveRunActionId, value: "7");

        await service.HandleBlockActionsAsync(payload, TestContext.Current.CancellationToken);

        var reply = Assert.Single(replies.Values);
        Assert.Equal(ErrorStrings.LeaveRun_RunEnded(), reply.Text);
    }

    private static ISender CreateJoinMediator(long optionGroupId)
    {
        var now = DateTimeOffset.UtcNow;
        var mediator = A.Fake<ISender>();

        A.CallTo(() => mediator.Send(A<GetObjectIdByLinkValueQuery>._, A<CancellationToken>._))
            .Returns((long?)5);
        A.CallTo(() => mediator.Send(A<GetUserQuery>._, A<CancellationToken>._))
            .Returns(new UserEntity { Id = 5, Username = "slack_U1", DisplayName = "steve", CreatedDate = now });
        A.CallTo(() => mediator.Send(A<GetRoomQuery>._, A<CancellationToken>._))
            .Returns(new RoomEntity { Id = 5, Name = "general", CreatedBy = 5, CreatedDate = now });
        A.CallTo(() => mediator.Send(A<GetCurrentRunQuery>._, A<CancellationToken>._))
            .Returns(CreateRun(roomId: 5));
        A.CallTo(() => mediator.Send(A<GetRunQuery>.That.Matches(q => q.RunId == 7), A<CancellationToken>._))
            .Returns(CreateRun(roomId: 5));
        A.CallTo(() => mediator.Send(A<GetOptionQuery>._, A<CancellationToken>._))
            .Returns(new OptionEntity
            {
                Id = 1,
                Name = "Earl Grey",
                Available = true,
                GroupId = optionGroupId,
                CreatedBy = 5,
                CreatedDate = now
            });
        A.CallTo(() => mediator.Send(A<GetUserOrderQuery>._, A<CancellationToken>._))
            .Returns((OrderEntity?)null);

        return mediator;
    }

    // Strict, so a reply anywhere but the response url fails the test
    private static ISlackApiClient CreateApiClient(Captured<SlashCommandResponse> replies)
    {
        var apiClient = A.Fake<ISlackApiClient>(o => o.Strict());

        A.CallTo(() => apiClient.PostResponseAsync(ResponseUrl, replies._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);

        return apiClient;
    }

    private static RunEntity CreateRun(long roomId, bool ended = false) => new()
    {
        Id = 7,
        RoomId = roomId,
        UserId = 5,
        GroupId = RunGroupId,
        StartTime = DateTimeOffset.UtcNow,
        Ended = ended,
        CreatedDate = DateTimeOffset.UtcNow
    };

    private static SlackService CreateService(ISlackApiClient apiClient, ISender? mediator = null,
        IIdGenerator<long>? idGenerator = null) => new(
        mediator ?? A.Fake<ISender>(o => o.Strict()),
        idGenerator ?? A.Fake<IIdGenerator<long>>(o => o.Strict()),
        A.Fake<IDistributedHash>(o => o.Strict()),
        TimeProvider.System,
        apiClient,
        NullLogger<SlackService>.Instance);

    private static BlockActionsPayload CreatePayload(string? responseUrl, string actionId,
        string value = JoinValue) => new()
    {
        Type = BlockActionsPayload.PayloadType,
        User = new InteractionUser { Id = "U1", Username = "steve" },
        Channel = new InteractionChannel { Id = "C1", Name = "general" },
        ResponseUrl = responseUrl,
        Actions = [new BlockAction { ActionId = actionId, Value = value }]
    };
}
