namespace TeaTime.Slack.Models.BlockKit;

using System.Text.Json.Serialization;

/// <summary>
/// See https://docs.slack.dev/reference/block-kit/composition-objects/text-object
/// </summary>
public class TextObject
{
    private TextObject(string type, string text, bool? emoji)
    {
        Type = type;
        Text = text;
        Emoji = emoji;
    }

    [JsonPropertyName("type")]
    public string Type { get; }

    [JsonPropertyName("text")]
    public string Text { get; }

    /// <summary>
    /// Only valid for <c>plain_text</c>
    /// </summary>
    [JsonPropertyName("emoji")]
    public bool? Emoji { get; }

    public static TextObject PlainText(string text) => new("plain_text", text, true);

    public static TextObject Markdown(string text) => new("mrkdwn", text, null);
}
