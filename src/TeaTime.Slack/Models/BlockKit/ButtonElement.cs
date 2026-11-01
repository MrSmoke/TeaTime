namespace TeaTime.Slack.Models.BlockKit;

using System.Text.Json.Serialization;

/// <summary>
/// See https://docs.slack.dev/reference/block-kit/block-elements/button-element
/// </summary>
public class ButtonElement
{
    public const int MaxTextLength = 75;

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

    /// <summary>
    /// <c>primary</c> or <c>danger</c>; omit for the default style
    /// </summary>
    [JsonPropertyName("style")]
    public string? Style { get; init; }
}
