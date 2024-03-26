namespace TeaTime.Common.Tests;

using System.Threading.Tasks;
using Services;
using Xunit;

public class GuidBasedStringGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_DoesNotReturnEmpty()
    {
        var generator = new GuidBasedStringGenerator();

        // Run 100 times to make sure
        for (var i = 0; i < 100; i++)
        {
            var value = await generator.GenerateAsync();

            Assert.NotNull(value);
            Assert.NotEmpty(value);
            Assert.Equal(24, value.Length);
        }
    }

    [Fact]
    public async Task GenerateAsync_DoesNotContainInvalidCharacters()
    {
        var generator = new GuidBasedStringGenerator();

        // Run 100 times to make sure
        for (var i = 0; i < 100; i++)
        {
            var value = await generator.GenerateAsync();

            Assert.Matches("^[a-z0-9]*$", value);
        }
    }
}
