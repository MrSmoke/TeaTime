namespace TeaTime.Slack.Configuration
{
    public class SlackOptions
    {
        public string? VerificationToken { get; set; }
        public SlackOAuthOptions? OAuth { get; set; }
    }
}
