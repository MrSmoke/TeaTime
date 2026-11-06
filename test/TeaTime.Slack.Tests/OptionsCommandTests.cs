namespace TeaTime.Slack.Tests;

using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Features.Options.Commands;
using Common.Features.RoomItemGroups.Queries;
using FakeItEasy;
using MediatR;
using Resources;
using Xunit;

public class OptionsCommandTests
{
    private const long OptionId = 1;

    // Adding a name that already exists ignoring case is rejected with a friendly reply
    [Fact]
    public async Task Add_DuplicateName_RepliesAlreadyExists()
    {
        var mediator = CreateMediator(available: true);

        var duplicate = new OptionException("Option earl grey already exists",
            OptionException.OptionExceptionReason.DuplicateName);

        A.CallTo(() => mediator.Send(A<CreateOptionCommand>._, A<CancellationToken>._)).Throws(duplicate);

        var reply = await RunAsync(mediator, "options add tea \"earl grey\"");

        Assert.Equal(ErrorStrings.AddOption_DuplicateName("earl grey", "tea"), reply);
    }

    // Removing an option matches its name ignoring case and replies with the stored name
    [Fact]
    public async Task Remove_OptionNameDifferentCase_DeletesOption()
    {
        var mediator = CreateMediator(available: true);

        var reply = await RunAsync(mediator, "options remove tea \"earl grey\"");

        Assert.Equal("Removed option *Earl Grey* from group *tea*", reply);
        AssertSent<DeleteOptionCommand>(mediator, c => c.OptionId == OptionId);
    }

    // Marking an option available matches its name ignoring case and replies with the stored name
    [Fact]
    public async Task Available_OptionNameDifferentCase_MarksOptionAvailable()
    {
        var mediator = CreateMediator(available: false);

        var reply = await RunAsync(mediator, "options available tea \"EARL grey\"");

        Assert.Equal("*Earl Grey* is now available", reply);
        AssertSent<SetOptionAvailabilityCommand>(mediator, c => c.OptionId == OptionId && c.Available);
    }

    // Marking an option unavailable matches its name ignoring case and replies with the stored name
    [Fact]
    public async Task Unavailable_OptionNameDifferentCase_MarksOptionUnavailable()
    {
        var mediator = CreateMediator(available: true);

        var reply = await RunAsync(mediator, "options unavailable tea \"EARL grey\"");

        Assert.Equal("*Earl Grey* is now unavailable", reply);
        AssertSent<SetOptionAvailabilityCommand>(mediator, c => c.OptionId == OptionId && !c.Available);
    }

    // The room has a "tea" group holding a single "Earl Grey" option
    private static IMediator CreateMediator(bool available)
    {
        var earlGrey = SlackTestData.CreateOption(OptionId, "Earl Grey", available);
        var group = SlackTestData.CreateGroup("tea", earlGrey);

        var mediator = A.Fake<IMediator>(o => o.Strict());

        A.CallTo(() => mediator.Send(A<GetRoomItemGroupByNameQuery>._, A<CancellationToken>._))
            .Returns(group);

        A.CallTo(() => mediator.Send(A<DeleteOptionCommand>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);

        A.CallTo(() => mediator.Send(A<SetOptionAvailabilityCommand>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);

        return mediator;
    }

    private static async Task<string?> RunAsync(IMediator mediator, string command)
    {
        var slackService = CommandTestHost.CreateSlackService();
        var idGenerator = CommandTestHost.CreateIdGenerator();

        await using var provider = CommandTestHost.CreateProvider(slackService, mediator, idGenerator);
        using var json = await CommandTestHost.RunJsonAsync(provider, command);

        return json.RootElement.GetProperty("text").GetString();
    }

    private static void AssertSent<TCommand>(IMediator mediator, Expression<Func<TCommand, bool>> predicate)
        where TCommand : IRequest
    {
        // Commands carry a State dictionary, so match on their values rather than record equality
        A.CallTo(() => mediator.Send(A<TCommand>.That.Matches(predicate), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
