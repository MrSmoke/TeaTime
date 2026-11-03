namespace TeaTime.Slack.Models.BlockKit;

using System.Text.Json.Serialization;

public class SectionBlock(TextObject text) : Block
{
    public override string Type => "section";

    [JsonPropertyName("text")]
    public TextObject Text { get; } = text;
}
