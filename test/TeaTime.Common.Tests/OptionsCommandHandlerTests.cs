namespace TeaTime.Common.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Exceptions;
using FakeItEasy;
using Features.Options;
using Features.Options.Commands;
using Models.Data;
using Xunit;

public class OptionsCommandHandlerTests
{
    private const long GroupId = 2;

    // Adding an option whose name matches an existing one ignoring case is rejected
    [Fact]
    public async Task Handle_Create_DuplicateNameDifferentCase_Throws()
    {
        var repository = CreateRepository();
        var handler = new OptionsCommandHandler(TimeProvider.System, repository);

        var command = new CreateOptionCommand(Id: 10, UserId: 1, GroupId: GroupId, Name: "earl GREY");

        var e = await Assert.ThrowsAsync<OptionException>(
            () => handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Equal(OptionException.OptionExceptionReason.DuplicateName, e.Reason);
        A.CallTo(() => repository.CreateAsync(A<OptionEntity>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    // A new name is saved as an available option
    [Fact]
    public async Task Handle_Create_NewName_SavesOption()
    {
        var saved = A.Captured<OptionEntity>();
        var repository = CreateRepository();
        A.CallTo(() => repository.CreateAsync(saved._, A<CancellationToken>._)).Returns(Task.CompletedTask);

        var handler = new OptionsCommandHandler(TimeProvider.System, repository);
        var command = new CreateOptionCommand(Id: 10, UserId: 1, GroupId: GroupId, Name: "Green");

        await handler.Handle(command, TestContext.Current.CancellationToken);

        var option = saved.GetLastValue();
        Assert.Equal(10, option.Id);
        Assert.Equal("Green", option.Name);
        Assert.True(option.Available);
    }

    // The group already holds an "Earl Grey" option
    private static IOptionsRepository CreateRepository()
    {
        var earlGrey = new OptionEntity
        {
            Id = 1,
            Name = "Earl Grey",
            Available = true,
            GroupId = GroupId,
            CreatedBy = 1,
            CreatedDate = DateTimeOffset.UnixEpoch
        };

        var repository = A.Fake<IOptionsRepository>(o => o.Strict());

        A.CallTo(() => repository.GetOptionsByGroupIdAsync(GroupId, A<CancellationToken>._))
            .Returns([earlGrey]);

        A.CallTo(() => repository.CreateAsync(A<OptionEntity>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);

        return repository;
    }
}
