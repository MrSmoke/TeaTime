namespace TeaTime.Slack.Models.Responses;

using System.Text.Json.Serialization;

public class OAuthTokenResponse : BaseResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("scope")]
    public required string Scope { get; init; }

    [JsonPropertyName("team")]
    public required SlackTeam Team { get; init; }

    [JsonPropertyName("incoming_webhook")]
    public required IncomingWebhook IncomingWebhook { get; init; }

    public object ToLogObject()
    {
        return new
        {
            AccessToken = Redact(AccessToken),
            Scope,
            TeamName = Redact(Team.Name),
            TeamId = Team.Id,
            IncomingWebhook.ChannelId,
            IncomingWebhook.Channel
        };

        static string? Redact(string? value) => string.IsNullOrWhiteSpace(value) ? value : "***";
    }
}
