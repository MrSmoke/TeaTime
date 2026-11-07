namespace TeaTime.Slack.Tests;

using Resources;
using Xunit;

public class CommandStringsTests
{
    [Theory]
    [InlineData("tea", "tea")]
    [InlineData("black tea", "\"black tea\"")]
    public void AddGroup_QuotesNamesWithSpaces(string groupName, string expected)
    {
        Assert.Equal($"You can create this group by typing `/teatime groups add {expected}`",
            CommandStrings.AddGroup(groupName));
    }

    [Fact]
    public void AddOption_QuotesNamesWithSpaces()
    {
        Assert.Equal("You can create this option by typing `/teatime options add \"black tea\" \"earl grey\"`",
            CommandStrings.AddOption("black tea", "earl grey"));
        Assert.Equal("You can add options to this group by typing `/teatime options add \"black tea\" {option}`",
            CommandStrings.AddOption("black tea"));
    }

    [Fact]
    public void JoinRound_IncludesOption()
    {
        Assert.Contains("`/teatime join {option}`", CommandStrings.JoinRound);
    }

    [Fact]
    public void RemoveBadArguments_ShowRemoveUsage()
    {
        Assert.Contains("/teatime options remove", ErrorStrings.RemoveOption_BadArguments());
        Assert.Contains("/teatime groups remove", ErrorStrings.RemoveGroup_BadArguments());
    }
}
