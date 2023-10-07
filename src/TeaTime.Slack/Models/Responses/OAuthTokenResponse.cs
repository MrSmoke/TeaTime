namespace TeaTime.Slack.Models.Responses
{
    using System.Text.Json.Serialization;

    public class OAuthTokenResponse : BaseResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("team_name")]
        public string TeamName { get; set; }

        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }

        [JsonPropertyName("incoming_webhook")]
        public OAuthTokenResponseIncomingWebhook? IncomingWebhook { get; set; }
    }
}
