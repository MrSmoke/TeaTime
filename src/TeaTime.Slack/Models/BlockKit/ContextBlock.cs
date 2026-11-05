namespace TeaTime.Slack.Models.BlockKit;

using System.Collections.Generic;

public class ContextBlock : Block
{
    public override string Type => "context";

    public List<TextObject> Elements { get; set; } = [];
}
