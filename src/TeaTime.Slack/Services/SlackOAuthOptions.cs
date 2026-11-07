namespace TeaTime.Slack.Services;

public class SlackOAuthOptions
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public bool Enabled { get; set; } = true;
}
