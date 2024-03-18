namespace TeaTime.Slack.Configuration
{
    using Services;

    public class SlackOptions
    {
        public SignedSecretsRequestVerifierOptions? RequestVerification { get; set; }
        public SlackOAuthOptions? OAuth { get; set; }
    }
}
