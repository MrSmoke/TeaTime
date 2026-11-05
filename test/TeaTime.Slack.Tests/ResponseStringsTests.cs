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
        const string expected = "Options for *tea*\n" +
                                "- Earl Grey\n" +
                                "- Green";

        Option[] options = [CreateOption("Earl Grey", available: true), CreateOption("Green", available: true)];

        Assert.Equal(expected, ResponseStrings.OptionList("tea", options));
    }

    // Unavailable options are listed separately after the available ones
    [Fact]
    public void OptionList_ListsUnavailableOptionsSeparately()
    {
        const string expected = "Options for *tea*\n" +
                                "- Earl Grey\n" +
                                "\n" +
                                "*Unavailable*\n" +
                                "- Green";

        Option[] options = [CreateOption("Green", available: false), CreateOption("Earl Grey", available: true)];

        Assert.Equal(expected, ResponseStrings.OptionList("tea", options));
    }

    private static Option CreateOption(string name, bool available) => new()
    {
        Id = 1,
        Name = name,
        Available = available,
        GroupId = 1,
        CreatedBy = 1,
        CreatedDate = DateTimeOffset.UnixEpoch
    };
}
