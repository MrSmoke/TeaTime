namespace TeaTime.Slack.Models.Responses;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

public class OAuthTokenResponse : BaseResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("team_name")]
    public string? TeamName { get; set; }

    [JsonPropertyName("team_id")]
    public string? TeamId { get; set; }

    [JsonPropertyName("incoming_webhook")]
    public OAuthTokenResponseIncomingWebhook? IncomingWebhook { get; set; }

    [MemberNotNullWhen(true, nameof(AccessToken))]
    [MemberNotNullWhen(true, nameof(Scope))]
    [MemberNotNullWhen(true, nameof(TeamName))]
    [MemberNotNullWhen(true, nameof(TeamId))]
    public bool ValidateProperties()
    {
        return !string.IsNullOrWhiteSpace(AccessToken) &&
               !string.IsNullOrWhiteSpace(Scope) &&
               !string.IsNullOrWhiteSpace(TeamName) &&
               !string.IsNullOrWhiteSpace(TeamId);
    }

    public object ToLogObject()
    {
        return new
        {
            AccessToken = Redact(AccessToken),
            Scope,
            TeamName = Redact(TeamName),
            TeamId
        };

        static string? Redact(string? value) => string.IsNullOrWhiteSpace(value) ? value : "***";
    }
}
