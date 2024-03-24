namespace TeaTime.Slack.Models.Responses;

using System.Text.Json.Serialization;

public class SlackTeam
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}
