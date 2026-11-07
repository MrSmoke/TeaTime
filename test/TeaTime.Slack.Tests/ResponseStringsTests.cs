namespace TeaTime.Slack.Tests;

using System;
using Common.Models.Data;
using Resources;
using Xunit;

public class ResponseStringsTests
{
    // Available options are listed under the group heading
    [Fact]
    public void OptionList_ListsEachOption()
    {
        const string expected = "*Available*\n" +
                                "- Earl Grey\n" +
                                "- Green";

        OptionEntity[] options = [CreateOption("Earl Grey", available: true), CreateOption("Green", available: true)];

        Assert.Equal(expected, ResponseStrings.OptionList("tea", options));
    }

    // Unavailable options are listed separately after the available ones
    [Fact]
    public void OptionList_ListsUnavailableOptionsSeparately()
    {
        const string expected = "*Available*\n" +
                                "- Earl Grey\n" +
                                "\n" +
                                "*Unavailable*\n" +
                                "- Green";

        OptionEntity[] options = [CreateOption("Green", available: false), CreateOption("Earl Grey", available: true)];

        Assert.Equal(expected, ResponseStrings.OptionList("tea", options));
    }

    // When every option is unavailable a note replaces the empty available list
    [Fact]
    public void OptionList_AllUnavailable_SaysNoneAvailable()
    {
        const string expected = "*Available*\n" +
                                "No options are currently available\n" +
                                "\n" +
                                "*Unavailable*\n" +
                                "- Green";

        OptionEntity[] options = [CreateOption("Green", available: false)];

        Assert.Equal(expected, ResponseStrings.OptionList("tea", options));
    }

    // Option names can't form mentions or links, and text Slack already escaped isn't escaped twice
    [Theory]
    [InlineData("<!channel>", "- &lt;!channel&gt;")]
    [InlineData("&lt;!channel&gt;", "- &lt;!channel&gt;")]
    [InlineData("Tea & Biscuits", "- Tea &amp; Biscuits")]
    [InlineData("Tea &amp; Biscuits", "- Tea &amp; Biscuits")]
    public void OptionList_EscapesOptionNames(string name, string expectedLine)
    {
        OptionEntity[] options = [CreateOption(name, available: true)];

        Assert.Equal("*Available*\n" + expectedLine, ResponseStrings.OptionList("tea", options));
    }

    private static OptionEntity CreateOption(string name, bool available) => new()
    {
        Id = 1,
        Name = name,
        Available = available,
        GroupId = 1,
        CreatedBy = 1,
        CreatedDate = DateTimeOffset.UnixEpoch
    };
}
