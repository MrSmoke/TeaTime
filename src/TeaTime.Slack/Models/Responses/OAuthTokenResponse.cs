namespace TeaTime.Slack.Models.Responses;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

public class OAuthTokenResponse : BaseResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("team")]
    public SlackTeam? Team { get; init; }

    [JsonPropertyName("incoming_webhook")]
    public OAuthTokenResponseIncomingWebhook? IncomingWebhook { get; set; }

    [MemberNotNullWhen(true, nameof(AccessToken))]
    [MemberNotNullWhen(true, nameof(Scope))]
    [MemberNotNullWhen(true, nameof(Team))]
    public bool ValidateProperties()
    {
        return !string.IsNullOrWhiteSpace(AccessToken) &&
               !string.IsNullOrWhiteSpace(Scope) &&
               Team is not null;
    }

    public object ToLogObject()
    {
        return new
        {
            AccessToken = Redact(AccessToken),
            Scope,
            TeamName = Redact(Team?.Name),
            TeamId = Team?.Id
        };

        static string? Redact(string? value) => string.IsNullOrWhiteSpace(value) ? value : "***";
    }
}
