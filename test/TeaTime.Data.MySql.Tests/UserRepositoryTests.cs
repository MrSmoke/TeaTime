namespace TeaTime.Data.MySql.Tests;

using System.Threading.Tasks;
using Common.Models.Data;
using Repositories;
using Xunit;

public class UserRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;

    public UserRepositoryTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    [Fact(Skip = "Requires local mysql")]
    public async Task CreateAsync_GetAsync()
    {
        var repo = new UserRepository(_databaseFixture.ConnectionFactory);

        var user = new User
        {
            Id = TestData.Int64(),
            Username = TestData.String(30),
            CreatedDate = TestData.MySqlNow(),
            DisplayName = TestData.String(30)
        };

        await repo.CreateAsync(user);

        var result = await repo.GetAsync(user.Id);

        Assert.NotNull(result);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.DisplayName, result.DisplayName);
        Assert.Equal(user.CreatedDate, result.CreatedDate);
    }
}
