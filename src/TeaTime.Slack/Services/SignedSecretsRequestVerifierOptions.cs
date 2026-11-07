namespace TeaTime.Slack.Services;

using System;

public class SignedSecretsRequestVerifierOptions
{
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);
    public string? SigningSecret { get; set; }
    public bool Enabled { get; set; } = true;
}
