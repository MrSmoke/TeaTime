namespace TeaTime.Slack.Services;

using System;

public class SignedSecretsRequestVerifierOptions
{
    public TimeSpan Something { get; init; }
    public required string SigningSecret { get; set; }
}
