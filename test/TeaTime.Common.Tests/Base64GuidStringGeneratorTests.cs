namespace TeaTime.Common.Tests;

using System.Threading.Tasks;
using Services;
using Xunit;

public class Base64GuidStringGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_DoesNotReturnEmpty()
    {
        var generator = new Base64GuidStringGenerator();
        var value = await generator.GenerateAsync();

        Assert.NotNull(value);
        Assert.NotEmpty(value);
    }

    [Fact]
    public async Task GenerateAsync_DoesNotContainInvalidCharacters()
    {
        var generator = new Base64GuidStringGenerator();

        // Run 100 times to make sure
        for (var i = 0; i < 100; i++)
        {
            var value = await generator.GenerateAsync();

            // Make sure we dont contain any invalid characters
            Assert.DoesNotContain('=', value);
            Assert.DoesNotContain('+', value);
            Assert.DoesNotContain('/', value);
        }
    }
}
