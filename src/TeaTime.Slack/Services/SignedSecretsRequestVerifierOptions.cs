namespace TeaTime.Slack.Services;

using System;
using Microsoft.Extensions.Options;

public class SignedSecretsRequestVerifierOptions : IValidateOptions<SignedSecretsRequestVerifierOptions>
{
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromSeconds(10);
    public string? SigningSecret { get; set; }
    public bool Enabled { get; set; } = true;

    public ValidateOptionsResult Validate(string? name, SignedSecretsRequestVerifierOptions options)
    {
        if (!options.Enabled)
            return ValidateOptionsResult.Success;

        if (string.IsNullOrWhiteSpace(options.SigningSecret))
            return ValidateOptionsResult.Fail($"{nameof(SigningSecret)} is required");

        return ValidateOptionsResult.Success;
    }
}
