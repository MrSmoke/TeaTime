namespace TeaTime.Slack.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Common.Abstractions;
using Common.Collections;
using Common.Features.Links.Queries;
using Common.Features.Users.Commands;
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
                    r.Text == ErrorStrings.General() && r.Type == ResponseType.User)))
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
        A.CallTo(() => idGenerator.GenerateAsync()).Returns(10L);

        var service = CreateService(A.Fake<ISlackApiClient>(), mediator, idGenerator);

        var user = await service.GetOrCreateUser("U123", name);

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
        A.CallTo(() => idGenerator.GenerateAsync()).Returns(20L);

        var service = CreateService(A.Fake<ISlackApiClient>(), mediator, idGenerator);

        var room = await service.GetOrCreateRoom("C123", name, userId: 1);

        Assert.Equal("Slack Channel C123", room.Name);
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
        Actions = [new BlockAction { Type = "button", ActionId = actionId, Value = "1" }]
    };
}
