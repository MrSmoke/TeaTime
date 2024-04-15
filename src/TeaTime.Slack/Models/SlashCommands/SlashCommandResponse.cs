namespace TeaTime.Slack.Models.SlashCommands;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class SlashCommandResponse(string? text, ResponseType responseType)
{
    public SlashCommandResponse() : this(null, SlashCommands.ResponseType.Channel)
    {
    }

    public string? Text { get; init; } = text;

    [JsonPropertyName("replace_original")]
    public bool ReplaceOriginal { get; init; }

    [JsonIgnore]
    public ResponseType Type { get; init; } = responseType;

    public IEnumerable<Attachment>? Attachments { get; init; }

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
