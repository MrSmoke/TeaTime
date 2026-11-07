namespace TeaTime.Slack.Models.BlockKit;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class ActionsBlock : Block
{
    public const int MaxElements = 25;

    public override string Type => "actions";

    [JsonPropertyName("elements")]
    public List<ButtonElement> Elements { get; } = [];
}
