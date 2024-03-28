namespace TeaTime.Data.MySql.Tests.Repositories;

using System.Threading.Tasks;
using Common;
using Common.Models.Data;
using MySql.Repositories;
using Xunit;

public class RunRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;

    public RunRepositoryTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    //[Fact]
    [Fact(Skip = "Requires local mysql")]
    public async Task GetAsync_Returns()
    {
        // This is gross
        var userRepo = new UserRepository(_databaseFixture.ConnectionFactory);
        var user = new User
        {
            Id = TestData.NewInt64(),
            Username = TestData.NewString(),
            CreatedDate = TestData.MySqlNow(),
            DisplayName = TestData.NewString()
        };
        await userRepo.CreateAsync(user);

        var roomRepo = new RoomRepository(_databaseFixture.ConnectionFactory);
        var room = new Room
        {
            Id = TestData.NewInt64(),
            Name = TestData.NewString(),
            CreatedBy = user.Id,
            CreatedDate = TestData.MySqlNow()
        };
        await roomRepo.CreateAsync(room);

        var optionRepo = new OptionsRepository(_databaseFixture.ConnectionFactory, new DefaultSystemClock());
        var group = new RoomItemGroup
        {
            Id = TestData.NewInt64(),
            Name = TestData.NewString(),
            CreatedBy = user.Id,
            CreatedDate = TestData.MySqlNow(),
            RoomId = room.Id
        };
        await optionRepo.CreateGroupAsync(group);

        var option = new Option
        {
            Id = TestData.NewInt64(),
            Name = TestData.NewString(),
            CreatedBy = user.Id,
            CreatedDate = TestData.MySqlNow(),
            GroupId = group.Id
        };
        await optionRepo.CreateAsync(option);

        var runRepo = new RunRepository(_databaseFixture.ConnectionFactory);
        var run = new Run
        {
            Id = TestData.NewInt64(),
            CreatedDate = TestData.MySqlNow(),
            GroupId = group.Id,
            RoomId = room.Id,
            StartTime = TestData.MySqlNow(),
            UserId = user.Id,
            Ended = false
        };
        await runRepo.CreateAsync(run);

        // Ok setup done, now we can actually do the test
        var result = await runRepo.GetAsync(run.Id);

        Assert.NotNull(result);
        Assert.Equal(run, result);
    }
}
