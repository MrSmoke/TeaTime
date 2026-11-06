namespace TeaTime.Common.Tests;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using FakeItEasy;
using Features.RoomItemGroups;
using Features.RoomItemGroups.Queries;
using Models.Data;
using Xunit;

public class RoomItemGroupQueryHandlerTests
{
    private static readonly string[] UnsortedNames = ["Option 10", "option 9", "Apple", "Option 2", "banana"];

    private static readonly string[] SortedNames = ["Apple", "banana", "Option 2", "option 9", "Option 10"];

    private static readonly RoomItemGroup Group = new()
    {
        Id = 1,
        Name = "drinks",
        RoomId = 1,
        CreatedBy = 1,
        CreatedDate = DateTimeOffset.UnixEpoch
    };

    // Options are sorted by name ignoring case, with numbers compared by value so 9 comes before 10
    [Fact]
    public async Task Handle_GetById_SortsOptionsByName()
    {
        var repository = CreateRepository();
        A.CallTo(() => repository.GetGroupAsync(Group.Id, A<CancellationToken>._)).Returns(Group);

        var handler = new RoomItemGroupQueryHandler(repository);
        var query = new GetRoomItemGroupQuery(RoomId: 1, UserId: 1, GroupId: Group.Id);

        var model = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(model);
        Assert.Equal(SortedNames, model.Options.Select(o => o.Name));
    }

    // Lookup by name applies the same ordering as lookup by id
    [Fact]
    public async Task Handle_GetByName_SortsOptionsByName()
    {
        var repository = CreateRepository();
        A.CallTo(() => repository.GetGroupByNameAsync(Group.RoomId, Group.Name, A<CancellationToken>._))
            .Returns(Group);

        var handler = new RoomItemGroupQueryHandler(repository);
        var query = new GetRoomItemGroupByNameQuery(RoomId: Group.RoomId, UserId: 1, Name: Group.Name);

        var model = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(model);
        Assert.Equal(SortedNames, model.Options.Select(o => o.Name));
    }

    private static IOptionsRepository CreateRepository()
    {
        var options = UnsortedNames.Select((name, i) => new Option
        {
            Id = i + 1,
            Name = name,
            Available = true,
            GroupId = Group.Id,
            CreatedBy = 1,
            CreatedDate = DateTimeOffset.UnixEpoch
        });

        var repository = A.Fake<IOptionsRepository>();
        A.CallTo(() => repository.GetOptionsByGroupIdAsync(Group.Id, A<CancellationToken>._))
            .Returns(options.ToList());

        return repository;
    }
}
