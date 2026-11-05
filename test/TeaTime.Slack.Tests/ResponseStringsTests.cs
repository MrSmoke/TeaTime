namespace TeaTime.Slack.Tests;

using Resources;
using Xunit;

public class ResponseStringsTests
{
    // Each option name gets its own bolded line under the group heading
    [Fact]
    public void OptionList_ListsEachOption()
    {
        const string expected = "Options for *tea*\n" +
                                "- Earl Grey\n" +
                                "- Green";

        Assert.Equal(expected, ResponseStrings.OptionList("tea", ["Earl Grey", "Green"]));
    }
}
