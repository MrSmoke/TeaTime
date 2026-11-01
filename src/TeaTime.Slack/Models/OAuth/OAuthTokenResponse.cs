namespace TeaTime.Slack.Models.OAuth;

using System.Text.Json.Serialization;

/// <summary>
/// See https://docs.slack.dev/reference/methods/oauth.v2.access
/// </summary>
public class OAuthTokenResponse : BaseResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; init; }

    [JsonPropertyName("scope")]
    public string? Scope { get; init; }

    [JsonPropertyName("bot_user_id")]
    public string? BotUserId { get; init; }

    [JsonPropertyName("app_id")]
    public string? AppId { get; init; }

    /// <summary>
    /// Null for org-wide installs
    /// </summary>
    [JsonPropertyName("team")]
    public SlackTeam? Team { get; init; }

    [JsonPropertyName("enterprise")]
    public SlackEnterprise? Enterprise { get; init; }

    [JsonPropertyName("is_enterprise_install")]
    public bool IsEnterpriseInstall { get; init; }

    [JsonPropertyName("authed_user")]
    public SlackAuthedUser? AuthedUser { get; init; }

    /// <summary>
    /// Only set when token rotation is enabled
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Seconds until <see cref="AccessToken"/> expires; only set when token rotation is enabled
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int? ExpiresIn { get; init; }
}
