namespace TeaTime.Slack;

internal static class Constants
{
    internal static class CommandContextKeys
    {
        internal const string SlashCommand = "SLASHCOMMAND";
    }

    internal static class FieldKeys
    {
        internal const string TeamName = "team_name";
        internal const string AccessToken = "access_token";
        internal const string InstallTime = "install_time";
        internal const string WebhookUrl = "webhook:url";
        internal const string ChannelName = "webhook:channel_name";
    }

    internal static class RouteNames
    {
        public const string OauthCallback = "SlackOauthCallback";
    }
}
