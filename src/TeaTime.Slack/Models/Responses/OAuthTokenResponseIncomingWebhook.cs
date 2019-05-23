namespace TeaTime.Slack.Models.Responses
{
    using Newtonsoft.Json;

    public class OAuthTokenResponseIncomingWebhook
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("configuration_url")]
        public string ConfigurationUrl { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}