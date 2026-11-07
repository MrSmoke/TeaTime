namespace TeaTime.Data.MySql.Tests.Repositories;

using System;
using System.Threading.Tasks;
using Common.Models.Data;
using MySql.Repositories;
using Xunit;

public class RunRepositoryTests(DatabaseFixture databaseFixture) : IClassFixture<DatabaseFixture>
{
    //[Fact]
    [Fact(Skip = "Requires local mysql")]
    public async Task GetAsync_Returns()
    {
        // This is gross
        var userRepo = new UserRepository(databaseFixture.ConnectionFactory);
        var user = new UserEntity
        {
            Id = TestData.NewInt64(),
            Username = TestData.NewString(),
            CreatedDate = TestData.MySqlNow(),
            DisplayName = TestData.NewString()
        };
        await userRepo.CreateAsync(user, TestContext.Current.CancellationToken);

        var roomRepo = new RoomRepository(databaseFixture.ConnectionFactory);
        var room = new RoomEntity
        {
            Id = TestData.NewInt64(),
            Name = TestData.NewString(),
            CreatedBy = user.Id,
            CreatedDate = TestData.MySqlNow()
        };
        await roomRepo.CreateAsync(room, TestContext.Current.CancellationToken);

        var optionRepo = new OptionsRepository(databaseFixture.ConnectionFactory, TimeProvider.System);
        var group = new RoomItemGroupEntity
        {
            Id = TestData.NewInt64(),
            Name = TestData.NewString(),
            CreatedBy = user.Id,
            CreatedDate = TestData.MySqlNow(),
            RoomId = room.Id
        };
        await optionRepo.CreateGroupAsync(group, TestContext.Current.CancellationToken);

        var option = new OptionEntity
        {
            Id = TestData.NewInt64(),
            Name = TestData.NewString(),
            Available = true,
            CreatedBy = user.Id,
            CreatedDate = TestData.MySqlNow(),
            GroupId = group.Id
        };
        await optionRepo.CreateAsync(option, TestContext.Current.CancellationToken);

        var runRepo = new RunRepository(databaseFixture.ConnectionFactory);
        var run = new RunEntity
        {
            Id = TestData.NewInt64(),
            CreatedDate = TestData.MySqlNow(),
            GroupId = group.Id,
            RoomId = room.Id,
            StartTime = TestData.MySqlNow(),
            UserId = user.Id,
            Ended = false
        };
        await runRepo.CreateAsync(run, TestContext.Current.CancellationToken);

        // Ok setup done, now we can actually do the test
        var result = await runRepo.GetAsync(run.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(run, result);
    }
}
