namespace TeaTime.Slack.Resources;

internal static class CommandStrings
{
    internal static string StartRound = "Type `/teatime start` to start a new round!";
    internal static string JoinRound = "Type `/teatime join {option}` to join this round!";
    internal static string AddGroup(string groupName) => $"You can create this group by typing `/teatime groups add {StringHelpers.QuoteArgument(groupName)}`";
    internal static string AddOption() => "You can add options by typing `/teatime options add {group} {option}`";
    internal static string AddOption(string groupName) => $"You can add options to this group by typing `/teatime options add {StringHelpers.QuoteArgument(groupName)} {{option}}`";
    internal static string AddOption(string groupName, string optionName) => $"You can create this option by typing `/teatime options add {StringHelpers.QuoteArgument(groupName)} {StringHelpers.QuoteArgument(optionName)}`";
    internal static string RemoveOption() => "You can remove options by typing `/teatime options remove {group} {option}`";
    internal static string RemoveGroup() => "You can remove groups by typing `/teatime groups remove {group}`";
}
