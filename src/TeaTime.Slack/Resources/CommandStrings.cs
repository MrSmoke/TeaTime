namespace TeaTime.Slack.Resources
{
    internal static class CommandStrings
    {
        internal static string StartRound = "Type `/teatime {group}` to start a new round!";
        internal static string JoinRound = "Type `/teatime join` to join this round!";
        internal static string AddGroup(string groupName) => $"You can create this group by typing `/teatime groups add {groupName}`";
        internal static string AddOption() => "You can add options by typing `/teatime options add {group} {option}`";
        internal static string AddOption(string groupName) => $"You can add options to this group by typing `/teatime options add {groupName} {{option}}`";
        internal static string AddOption(string groupName, string optionName) => $"You can create this option by typing `/teatime options add {groupName} {optionName}`";
    }
}