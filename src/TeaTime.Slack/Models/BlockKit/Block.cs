namespace TeaTime.Slack.Models.BlockKit;

using System.Text.Json.Serialization;

/// <summary>
/// A Block Kit layout block. See https://docs.slack.dev/reference/block-kit/blocks
/// </summary>
[JsonDerivedType(typeof(SectionBlock))]
[JsonDerivedType(typeof(ActionsBlock))]
public abstract class Block
{
    [JsonPropertyName("type")]
    public abstract string Type { get; }
}
