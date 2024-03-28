namespace TeaTime.Slack.Models.Requests;

public class OAuthTokenRequest
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string Code { get; set; }
    public required string RedirectUri { get; set; }
}
