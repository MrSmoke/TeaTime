namespace TeaTime.Slack.Configuration
{
    public class SlackOAuthOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }

        public bool IsValid() => !string.IsNullOrWhiteSpace(ClientId)
                                 && !string.IsNullOrWhiteSpace(ClientSecret)
                                 && !string.IsNullOrWhiteSpace(RedirectUri);
    }
}