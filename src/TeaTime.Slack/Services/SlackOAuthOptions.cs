namespace TeaTime.Slack.Services
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.Options;

    public class SlackOAuthOptions : IValidateOptions<SlackOAuthOptions>
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public bool Enabled { get; set; } = true;

        [MemberNotNullWhen(true, nameof(ClientId))]
        [MemberNotNullWhen(true, nameof(ClientSecret))]
        public bool IsValid() => Validate(null, this).Succeeded;

        public ValidateOptionsResult Validate(string? name, SlackOAuthOptions options)
        {
            if (!options.Enabled)
                return ValidateOptionsResult.Success;

            if (string.IsNullOrWhiteSpace(options.ClientId))
                return ValidateOptionsResult.Fail("Slack ClientId is missing");

            if (string.IsNullOrWhiteSpace(options.ClientSecret))
                return ValidateOptionsResult.Fail("Slack ClientSecret is missing");

            return ValidateOptionsResult.Success;
        }
    }
}
