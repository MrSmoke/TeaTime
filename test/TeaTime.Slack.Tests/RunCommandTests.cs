namespace TeaTime.Slack.Tests;

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Features.RoomItemGroups.Models;
using Common.Features.RoomItemGroups.Queries;
using Common.Features.Runs.Commands;
using FakeItEasy;
using MediatR;
using Resources;
using Xunit;

public class RunCommandTests
{
    // Only available options get a join button when a round starts
    [Fact]
    public async Task Start_SomeOptionsUnavailable_OnlyShowsAvailableOptions()
    {
        var group = SlackTestData.CreateGroup("tea",
            SlackTestData.CreateOption(1, "Earl Grey"),
            SlackTestData.CreateOption(2, "Green", available: false));

        var mediator = CreateMediator(group);

        var reply = await RunAsync(mediator, "start tea");

        Assert.Contains(BlockBuilder.JoinOptionActionIdPrefix + "1", reply);
        Assert.DoesNotContain(BlockBuilder.JoinOptionActionIdPrefix + "2", reply);
        A.CallTo(() => mediator.Send(A<StartRunCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    // A group whose options are all unavailable can't start a round
    [Fact]
    public async Task Start_AllOptionsUnavailable_RepliesNoOptions()
    {
        var group = SlackTestData.CreateGroup("tea", SlackTestData.CreateOption(2, "Green", available: false));

        var mediator = CreateMediator(group);

        using var reply = JsonDocument.Parse(await RunAsync(mediator, "start tea"));

        Assert.Equal(ErrorStrings.StartRun_GroupNoOptions("tea"), reply.RootElement.GetProperty("text").GetString());
        A.CallTo(() => mediator.Send(A<StartRunCommand>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    private static IMediator CreateMediator(RoomItemGroupModel group)
    {
        var mediator = A.Fake<IMediator>(o => o.Strict());

        A.CallTo(() => mediator.Send(A<GetRoomItemGroupByNameQuery>._, A<CancellationToken>._))
            .Returns(group);

        A.CallTo(() => mediator.Send(A<StartRunCommand>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);

        return mediator;
    }

    private static async Task<string> RunAsync(IMediator mediator, string command)
    {
        var slackService = CommandTestHost.CreateSlackService();
        var idGenerator = CommandTestHost.CreateIdGenerator();

        await using var provider = CommandTestHost.CreateProvider(slackService, mediator, idGenerator);

        return await CommandTestHost.RunAsync(provider, command);
    }
}
