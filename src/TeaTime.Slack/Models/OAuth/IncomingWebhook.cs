namespace TeaTime.Slack.Models.OAuth;

using System.Text.Json.Serialization;

public class IncomingWebhook
{
    [JsonPropertyName("channel")]
    public required string Channel { get; init; }

    [JsonPropertyName("channel_id")]
    public required string ChannelId { get; init; }

    [JsonPropertyName("url")]
    public required string Url { get; init; }
}
