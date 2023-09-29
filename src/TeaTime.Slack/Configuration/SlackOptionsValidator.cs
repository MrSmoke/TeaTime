namespace TeaTime.Slack.Configuration
{
    using Microsoft.Extensions.Options;

    public class SlackOptionsValidator : IValidateOptions<SlackOptions>
    {
        public ValidateOptionsResult Validate(string? name, SlackOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.VerificationToken))
                return ValidateOptionsResult.Fail("Slack VerificationToken is missing");

            // Validate oauth
            if (options.OAuth != null)
            {
                var oauth = options.OAuth;

                if (string.IsNullOrWhiteSpace(oauth.ClientId))
                    return ValidateOptionsResult.Fail("Slack ClientId is missing");

                if (string.IsNullOrWhiteSpace(oauth.ClientSecret))
                    return ValidateOptionsResult.Fail("Slack ClientSecret is missing");

                if (string.IsNullOrWhiteSpace(oauth.RedirectUri))
                    return ValidateOptionsResult.Fail("Slack RedirectUri is missing");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
