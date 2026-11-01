namespace TeaTime.Slack.Models.SlashCommands;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BlockKit;

/// <summary>
/// Message payload for slash command replies and <c>response_url</c> posts.
/// See https://docs.slack.dev/interactivity/handling-user-interaction
/// </summary>
public class SlashCommandResponse(string? text, ResponseType responseType)
{
    public SlashCommandResponse() : this(null, SlashCommands.ResponseType.Channel)
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

    [JsonPropertyName("delete_original")]
    public bool DeleteOriginal { get; init; }

    [JsonPropertyName("thread_ts")]
    public string? ThreadTs { get; init; }

    [JsonIgnore]
    public ResponseType Type { get; init; } = responseType;

    [JsonPropertyName("response_type")]
    public string ResponseType
    {
        get
        {
            return Type switch
            {
                SlashCommands.ResponseType.Channel => "in_channel",
                SlashCommands.ResponseType.User => "ephemeral",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
