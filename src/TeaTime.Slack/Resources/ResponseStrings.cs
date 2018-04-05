namespace TeaTime.Slack.Resources
{
    public static class ResponseStrings
    {
        internal static string AddedOptionToGroup(string optionName, string groupName) =>
            $"Added option *{optionName}* to group *{groupName}*";

        internal static string RunStarted(string slackUserId) =>
            $"{StringHelpers.SlackUserId(slackUserId)} wants tea!";

        internal static string RunUserJoined(string slackUserId) =>
            $"{StringHelpers.SlackUserId(slackUserId)} has joined this round!";

        internal static string GroupAdded(string groupName) => $"*{groupName}* created!";
    }
}
