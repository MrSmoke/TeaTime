namespace TeaTime.Slack.Models.OAuth;

using System.Text.Json.Serialization;

public class OAuthTokenResponse : BaseResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("scope")]
    public required string Scope { get; init; }

    [JsonPropertyName("team")]
    public required SlackTeam Team { get; init; }
}
