namespace TeaTime.Slack.Configuration
{
    using System.Diagnostics.CodeAnalysis;

    public class SlackOAuthOptions
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? RedirectUri { get; set; }

        [MemberNotNullWhen(true, nameof(ClientId))]
        [MemberNotNullWhen(true, nameof(ClientSecret))]
        [MemberNotNullWhen(true, nameof(RedirectUri))]
        public bool IsValid() => !string.IsNullOrWhiteSpace(ClientId)
                                 && !string.IsNullOrWhiteSpace(ClientSecret)
                                 && !string.IsNullOrWhiteSpace(RedirectUri);
    }
}
