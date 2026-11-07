namespace TeaTime.Data.MySql.Tests.Repositories;

using System.Threading.Tasks;
using TeaTime.Common.Models.Data;
using TeaTime.Data.MySql.Repositories;
using Xunit;

public class UserRepositoryTests(DatabaseFixture databaseFixture) : IClassFixture<DatabaseFixture>
{
    //[Fact]
    [Fact(Skip = "Requires local mysql")]
    public async Task CreateAsync_GetAsync()
    {
        var repo = new UserRepository(databaseFixture.ConnectionFactory);

        var user = new UserEntity
        {
            Id = TestData.NewInt64(),
            Username = TestData.NewString(),
            CreatedDate = TestData.MySqlNow(),
            DisplayName = TestData.NewString()
        };

        await repo.CreateAsync(user, TestContext.Current.CancellationToken);

        var result = await repo.GetAsync(user.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(user, result);
    }
}
