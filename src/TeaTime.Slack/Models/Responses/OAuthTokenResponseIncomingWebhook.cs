namespace TeaTime.Slack.Models.Responses
{
    using System.Text.Json.Serialization;

    public class OAuthTokenResponseIncomingWebhook
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        [JsonPropertyName("configuration_url")]
        public string ConfigurationUrl { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
