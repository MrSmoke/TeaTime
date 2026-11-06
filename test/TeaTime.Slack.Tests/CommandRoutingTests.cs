namespace TeaTime.Slack.Tests;

using System;
using System.Linq;
using System.Threading.Tasks;
using CommandRouter.Exceptions;
using CommandRouter.Extensions;
using CommandRouter.Routing;
using Microsoft.Extensions.DependencyInjection;
using Resources;
using Xunit;

public class CommandRoutingTests
{
    public static TheoryData<string, string> MissingArgumentCommands => new()
    {
        { "options add tea", ErrorStrings.AddOption_BadArguments() },
        { "options add", ErrorStrings.AddOption_BadArguments() },
        { "options remove tea", ErrorStrings.RemoveOption_BadArguments() },
        { "options list", ErrorStrings.ListOptions_BadArguments() },
        { "options available tea", ErrorStrings.SetOptionAvailability_BadArguments(available: true) },
        { "options unavailable tea", ErrorStrings.SetOptionAvailability_BadArguments(available: false) },
        { "groups remove", ErrorStrings.RemoveGroup_BadArguments() }
    };

    [Fact]
    public void AddCommandRouter_RegistersEverySlackCommand()
    {
        using var provider = CommandTestHost.CreateProvider();

        provider.ValidateCommandRouter();

        Assert.Equal([
                "end",
                "groups add",
                "groups remove",
                "help",
                "illmake",
                "join",
                "leave",
                "options add",
                "options available",
                "options list",
                "options remove",
                "options unavailable",
                "start"
            ],
            provider.GetRequiredService<ICommandTable>().Keys.Order(StringComparer.Ordinal));
    }

    [Theory]
    [MemberData(nameof(MissingArgumentCommands))]
    public async Task RunAsync_MissingOptionalArgument_RepliesWithBadArguments(string command, string expected)
    {
        using var provider = CommandTestHost.CreateProvider();

        using var json = await CommandTestHost.RunJsonAsync(provider, command);

        Assert.Equal(expected, json.RootElement.GetProperty("text").GetString());
        Assert.Equal("ephemeral", json.RootElement.GetProperty("response_type").GetString());
    }

    [Theory]
    [InlineData("options add tea earl grey")]
    [InlineData("start earl grey")]
    [InlineData("help me")]
    [InlineData("groups add")]
    [InlineData("join")]
    public async Task RunAsync_ExtraOrMissingRequiredArgument_ThrowsParameterBindingException(string command)
    {
        using var provider = CommandTestHost.CreateProvider();

        await Assert.ThrowsAsync<ParameterBindingException>(() => CommandTestHost.RunAsync(provider, command));
    }

    [Fact]
    public async Task RunAsync_Help_ReturnsHelpText()
    {
        using var provider = CommandTestHost.CreateProvider();

        var help = await CommandTestHost.RunAsync(provider, "help");

        Assert.StartsWith("*Commands*", help);
        Assert.Contains("`/teatime join {option}`", help);
        Assert.Contains("double quotes", help);
    }
}
