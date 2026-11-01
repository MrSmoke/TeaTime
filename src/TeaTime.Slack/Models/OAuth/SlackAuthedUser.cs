namespace TeaTime.Slack.Models.OAuth;

using System.Text.Json.Serialization;

public class SlackAuthedUser
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("scope")]
    public string? Scope { get; init; }

    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; init; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }

    [JsonPropertyName("expires_in")]
    public int? ExpiresIn { get; init; }
}
