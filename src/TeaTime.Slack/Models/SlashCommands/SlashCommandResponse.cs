namespace TeaTime.Slack.Models.SlashCommands;

using System.Collections.Generic;
using System.Text.Json.Serialization;
using BlockKit;

/// <summary>
/// Message payload for slash command replies and <c>response_url</c> posts.
/// See https://docs.slack.dev/interactivity/handling-user-interaction
/// </summary>
public class SlashCommandResponse(string? text, ResponseType responseType)
{
    public SlashCommandResponse() : this(null, ResponseType.Channel)
    {
    }

    /// <summary>
    /// Message text, or the notification fallback when <see cref="Blocks"/> are set
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; } = text;

    [JsonPropertyName("blocks")]
    public IEnumerable<Block>? Blocks { get; init; }

    [JsonPropertyName("replace_original")]
    public bool ReplaceOriginal { get; init; }

    [JsonPropertyName("response_type")]
    public ResponseType Type { get; init; } = responseType;
}
