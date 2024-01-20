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
            Id = TestData.Int64(),
            Username = TestData.String(),
            CreatedDate = TestData.MySqlNow(),
            DisplayName = TestData.String()
        };

        await repo.CreateAsync(user);

        var result = await repo.GetAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user, result);
    }
}
