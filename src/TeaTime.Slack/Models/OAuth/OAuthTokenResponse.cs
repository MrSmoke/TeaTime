namespace TeaTime.Slack.Models.OAuth;

using System.Text.Json.Serialization;

/// <summary>
/// See https://docs.slack.dev/reference/methods/oauth.v2.access
/// </summary>
public class OAuthTokenResponse : BaseResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }

    [JsonPropertyName("scope")]
    public string? Scope { get; init; }

    /// <summary>
    /// Null for org-wide installs
    /// </summary>
    [JsonPropertyName("team")]
    public SlackTeam? Team { get; init; }

    [JsonPropertyName("is_enterprise_install")]
    public bool IsEnterpriseInstall { get; init; }
}
