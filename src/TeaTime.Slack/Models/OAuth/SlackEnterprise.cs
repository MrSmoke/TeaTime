namespace TeaTime.Slack.Models.OAuth;

using System.Text.Json.Serialization;

public class SlackEnterprise
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
