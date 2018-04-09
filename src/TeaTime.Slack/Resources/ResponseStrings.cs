namespace TeaTime.Slack.Resources
{
    internal static class ResponseStrings
    {
        internal static string AddedOptionToGroup(string optionName, string groupName) =>
            $"Added option *{optionName}* to group *{groupName}*";

        internal static string RunStarted(string slackUserId, string groupName) =>
            $"{StringHelpers.SlackUserId(slackUserId)} wants {groupName}!";

        internal static string RunUserJoined(string slackUserId) =>
            $"{StringHelpers.SlackUserId(slackUserId)} has joined this round!";

        internal static string GroupAdded(string groupName) => $"Group *{groupName}* created!";
    }
}
