namespace TeaTime.Slack.Models.BlockKit;

using System.Text.Json.Serialization;

/// <summary>
/// A Block Kit layout block. See https://docs.slack.dev/reference/block-kit/blocks
/// </summary>
[JsonDerivedType(typeof(ActionsBlock))]
[JsonDerivedType(typeof(ContextBlock))]
[JsonDerivedType(typeof(DividerBlock))]
[JsonDerivedType(typeof(SectionBlock))]
public abstract class Block
{
    [JsonPropertyName("type")]
    public abstract string Type { get; }
}
