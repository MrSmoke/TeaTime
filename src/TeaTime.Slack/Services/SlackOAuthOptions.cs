namespace TeaTime.Slack.Services
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.Options;

    public class SlackOAuthOptions : IValidateOptions<SlackOAuthOptions>
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? RedirectUri { get; set; }
        public bool Enabled { get; set; } = true;

        [MemberNotNullWhen(true, nameof(ClientId))]
        [MemberNotNullWhen(true, nameof(ClientSecret))]
        [MemberNotNullWhen(true, nameof(RedirectUri))]
        public bool IsValid() => !string.IsNullOrWhiteSpace(ClientId)
                                 && !string.IsNullOrWhiteSpace(ClientSecret)
                                 && !string.IsNullOrWhiteSpace(RedirectUri);

        public ValidateOptionsResult Validate(string? name, SlackOAuthOptions options)
        {
            if (!options.Enabled)
                return ValidateOptionsResult.Success;

            if (string.IsNullOrWhiteSpace(options.ClientId))
                return ValidateOptionsResult.Fail("Slack ClientId is missing");

            if (string.IsNullOrWhiteSpace(options.ClientSecret))
                return ValidateOptionsResult.Fail("Slack ClientSecret is missing");

            if (string.IsNullOrWhiteSpace(options.RedirectUri))
                return ValidateOptionsResult.Fail("Slack RedirectUri is missing");

            return ValidateOptionsResult.Success;
        }
    }
}
