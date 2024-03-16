namespace TeaTime.Slack.Configuration
{
    public class SlackOptions
    {
        public string? SigningSecret { get; set; }
        public SlackOAuthOptions? OAuth { get; set; }
    }
}
