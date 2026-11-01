namespace TeaTime.Slack.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CommandRouter;
    using CommandRouter.Exceptions;
    using CommandRouter.Extensions;
    using CommandRouter.Routing;
    using Common.Abstractions;
    using FakeItEasy;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Resources;
    using Services;
    using Xunit;

    public class CommandRoutingTests
    {
        [Fact]
        public void AddCommandRouter_RegistersEverySlackCommand()
        {
            using var provider = CreateProvider();

            provider.ValidateCommandRouter();

            Assert.Equal(
                ["", "end", "groups add", "groups remove", "help", "illmake", "join", "options add", "options remove"],
                provider.GetRequiredService<ICommandTable>().Keys.Order(StringComparer.Ordinal));
        }

        public static TheoryData<string, string> MissingArgumentCommands => new()
        {
            { "options add tea", ErrorStrings.AddOption_BadArguments() },
            { "options add", ErrorStrings.AddOption_BadArguments() },
            { "options remove tea", ErrorStrings.RemoveOption_BadArguments() },
            { "groups remove", ErrorStrings.RemoveGroup_BadArguments() }
        };

        [Theory]
        [MemberData(nameof(MissingArgumentCommands))]
        public async Task RunAsync_MissingOptionalArgument_RepliesWithBadArguments(string command, string expected)
        {
            using var provider = CreateProvider();

            // Check the JSON Slack receives, since Type isn't serialised
            using var json = JsonDocument.Parse(await RunAsync(provider, command));

            Assert.Equal(expected, json.RootElement.GetProperty("text").GetString());
            Assert.Equal("ephemeral", json.RootElement.GetProperty("response_type").GetString());
        }

        [Theory]
        [InlineData("options add tea earl grey")]
        [InlineData("earl grey")]
        [InlineData("help me")]
        [InlineData("groups add")]
        [InlineData("join")]
        public async Task RunAsync_ExtraOrMissingRequiredArgument_ThrowsParameterBindingException(string command)
        {
            using var provider = CreateProvider();

            await Assert.ThrowsAsync<ParameterBindingException>(() => RunAsync(provider, command));
        }

        [Fact]
        public async Task RunAsync_Help_ReturnsHelpText()
        {
            using var provider = CreateProvider();

            var help = await RunAsync(provider, "help");

            Assert.StartsWith("*Commands*", help);
            Assert.Contains("`/teatime join {option}`", help);
            Assert.Contains("double quotes", help);
        }

        private static ServiceProvider CreateProvider()
        {
            var services = new ServiceCollection();
            services.AddCommandRouter(typeof(ServiceCollectionExtension).Assembly);
            services.AddLogging();

            // Strict, so a test fails if a command touches any of these
            services.AddSingleton(A.Fake<ISlackService>(o => o.Strict()));
            services.AddSingleton(A.Fake<IMediator>(o => o.Strict()));
            services.AddSingleton(A.Fake<IIdGenerator<long>>(o => o.Strict()));
            services.AddSingleton(TimeProvider.System);

            return services.BuildServiceProvider();
        }

        private static async Task<string> RunAsync(ServiceProvider provider, string command)
        {
            var result = await provider.GetRequiredService<ICommandRunner>().RunAsync(command);

            using var stream = new MemoryStream();
            await result.ExecuteAsync(stream);

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
