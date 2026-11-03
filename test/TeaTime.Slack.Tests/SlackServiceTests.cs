namespace TeaTime.Slack.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Common.Abstractions;
using Common.Collections;
using Common.Features.Links.Queries;
using Common.Features.Options.Queries;
using Common.Features.Orders.Commands;
using Common.Features.Orders.Queries;
using Common.Features.Rooms.Queries;
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

    [Fact]
    public async Task HandleBlockActions_JoinFails_RepliesEphemerallyViaResponseUrl()
    {
        var apiClient = A.Fake<ISlackApiClient>();
        var service = CreateService(apiClient);

        // No join action, so the join fails before touching the domain
        await service.HandleBlockActionsAsync(CreatePayload(ResponseUrl, actionId: "something_else"));

        A.CallTo(() => apiClient.PostResponseAsync(ResponseUrl,
                A<SlashCommandResponse>.That.Matches(r =>
                    r.Text == ErrorStrings.General() && r.Type == ResponseType.User), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleBlockActions_NoResponseUrl_Ignored()
    {
        var apiClient = A.Fake<ISlackApiClient>(o => o.Strict());
        var service = CreateService(apiClient);

        await service.HandleBlockActionsAsync(CreatePayload(null, actionId: "join_option_1"));
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

        var user = await service.GetOrCreateUserAsync("U123", name);

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

        var room = await service.GetOrCreateRoomAsync("C123", name, userId: 1);

        Assert.Equal("Slack Channel C123", room.Name);
    }

    [Fact]
    public async Task HandleBlockActions_OptionFromAnotherGroup_RepliesUnknownAndDoesNotOrder()
    {
        var apiClient = A.Fake<ISlackApiClient>();
        var mediator = CreateJoinMediator(optionGroupId: 200);
        var service = CreateService(apiClient, mediator);

        await service.HandleBlockActionsAsync(CreatePayload(ResponseUrl, actionId: "join_option_1"));

        A.CallTo(() => apiClient.PostResponseAsync(ResponseUrl,
                A<SlashCommandResponse>.That.Matches(r => r.Text == ErrorStrings.OptionUnknown()),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        // Sent via a BaseCommand variable, so match on that type
        A.CallTo(() => mediator.Send(A<BaseCommand>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task HandleBlockActions_OptionFromRunGroup_CreatesOrder()
    {
        var apiClient = A.Fake<ISlackApiClient>();
        var mediator = CreateJoinMediator(optionGroupId: RunGroupId);
        var idGenerator = A.Fake<IIdGenerator<long>>();
        A.CallTo(() => idGenerator.GenerateAsync(A<CancellationToken>._)).Returns(30L);
        var service = CreateService(apiClient, mediator, idGenerator);

        await service.HandleBlockActionsAsync(CreatePayload(ResponseUrl, actionId: "join_option_1"));

        // Sent via a BaseCommand variable, so match on that type
        A.CallTo(() => mediator.Send(A<BaseCommand>.That.Matches(c => c is CreateOrderCommand && ((CreateOrderCommand)c).OptionId == 1),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => apiClient.PostResponseAsync(A<string>._, A<SlashCommandResponse>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    private const long RunGroupId = 100;

    private static ISender CreateJoinMediator(long optionGroupId)
    {
        var now = DateTimeOffset.UtcNow;
        var mediator = A.Fake<ISender>();

        A.CallTo(() => mediator.Send(A<GetObjectIdByLinkValueQuery>._, A<CancellationToken>._))
            .Returns((long?)5);
        A.CallTo(() => mediator.Send(A<GetUserQuery>._, A<CancellationToken>._))
            .Returns(new User { Id = 5, Username = "slack_U1", DisplayName = "steve", CreatedDate = now });
        A.CallTo(() => mediator.Send(A<GetRoomQuery>._, A<CancellationToken>._))
            .Returns(new Room { Id = 5, Name = "general", CreatedBy = 5, CreatedDate = now });
        A.CallTo(() => mediator.Send(A<GetCurrentRunQuery>._, A<CancellationToken>._))
            .Returns(new Run
            {
                Id = 7,
                RoomId = 5,
                UserId = 5,
                GroupId = RunGroupId,
                StartTime = now,
                CreatedDate = now
            });
        A.CallTo(() => mediator.Send(A<GetOptionQuery>._, A<CancellationToken>._))
            .Returns(new Option { Id = 1, Name = "Earl Grey", GroupId = optionGroupId, CreatedBy = 5, CreatedDate = now });
        A.CallTo(() => mediator.Send(A<GetUserOrderQuery>._, A<CancellationToken>._))
            .Returns((Order?)null);

        return mediator;
    }

    private static SlackService CreateService(ISlackApiClient apiClient, ISender? mediator = null,
        IIdGenerator<long>? idGenerator = null) => new(
        mediator ?? A.Fake<ISender>(o => o.Strict()),
        idGenerator ?? A.Fake<IIdGenerator<long>>(o => o.Strict()),
        A.Fake<IDistributedHash>(o => o.Strict()),
        TimeProvider.System,
        apiClient,
        NullLogger<SlackService>.Instance);

    private static BlockActionsPayload CreatePayload(string? responseUrl, string actionId) => new()
    {
        Type = BlockActionsPayload.PayloadType,
        User = new InteractionUser { Id = "U1", Username = "steve" },
        Channel = new InteractionChannel { Id = "C1", Name = "general" },
        ResponseUrl = responseUrl,
        Actions = [new BlockAction { ActionId = actionId, Value = "1" }]
    };
}
