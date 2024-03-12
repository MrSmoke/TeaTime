namespace TeaTime.Data.MySql.Tests.Repositories;

using System.Threading.Tasks;
using TeaTime.Common.Models.Data;
using TeaTime.Data.MySql.Repositories;
using Xunit;

public class UserRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;

    public UserRepositoryTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    //[Fact]
    [Fact(Skip = "Requires local mysql")]
    public async Task CreateAsync_GetAsync()
    {
        var repo = new UserRepository(_databaseFixture.ConnectionFactory);

        var user = new User
        {
            Id = TestData.NewInt64(),
            Username = TestData.NewString(),
            CreatedDate = TestData.MySqlNow(),
            DisplayName = TestData.NewString()
        };

        await repo.CreateAsync(user);

        var result = await repo.GetAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user, result);
    }
}
