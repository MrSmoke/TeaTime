namespace TeaTime.Slack.Models.BlockKit;

using System.Text.Json.Serialization;

/// <summary>
/// See https://docs.slack.dev/reference/block-kit/block-elements/button-element
/// </summary>
public class ButtonElement
{
    [JsonPropertyName("type")]
    public string Type { get; } = "button";

    [JsonPropertyName("text")]
    public required TextObject Text { get; init; }

    /// <summary>
    /// Must be unique within the containing block
    /// </summary>
    [JsonPropertyName("action_id")]
    public required string ActionId { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }
}
