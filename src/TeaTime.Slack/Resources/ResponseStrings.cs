namespace TeaTime.Slack.Resources
{
    internal static class ResponseStrings
    {
        internal static string OptionAddedToGroup(string optionName, string groupName) =>
            $"Added option *{optionName}* to group *{groupName}*";

        internal static string OptionRemoved(string optionName, string groupName) =>
            $"Removed option *{optionName}* from group *{groupName}*";

        internal static string RunStarted(string slackUserId, string groupName) =>
            $"{StringHelpers.SlackUserId(slackUserId)} wants {groupName}!";

        internal static string RunUserJoined(string slackUserId) =>
            $"{StringHelpers.SlackUserId(slackUserId)} has joined this round!";

        internal static string RunUserOrderChanged(string from, string to) =>
            $"You have changed your order: {from} -> {to}";

        internal static string GroupAdded(string groupName) => $"Group *{groupName}* created!";

        internal static string GroupRemoved(string groupName) => $"Group *{groupName}* has been removed.";

        internal static string IllMake(string slackUserId) =>
            $"{StringHelpers.SlackUserId(slackUserId)} has volunteered to make this round!";
    }
}
