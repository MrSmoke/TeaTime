namespace TeaTime.Slack.Models.BlockKit;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class ContextBlock : Block
{
    public override string Type => "context";

    [JsonPropertyName("elements")]
    public List<TextObject> Elements { get; } = [];
}
