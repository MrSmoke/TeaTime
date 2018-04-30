namespace TeaTime.Slack.Resources
{
    internal static class StringHelpers
    {
        internal static string SlackUserId(string id)
        {
            return $"<@{id}>";
        }
    }
}