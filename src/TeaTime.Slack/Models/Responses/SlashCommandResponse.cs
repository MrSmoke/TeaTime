namespace TeaTime.Slack.Models.Responses;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class SlashCommandResponse(string? text, ResponseType responseType)
{
    public SlashCommandResponse() : this(null, Responses.ResponseType.Channel)
    {
    }

    public string? Text { get; set; } = text;

    [JsonPropertyName("replace_original")]
    public bool ReplaceOriginal { get; set; }

    [JsonIgnore]
    public ResponseType Type { get; set; } = responseType;

    public IEnumerable<Attachment>? Attachments { get; set; }

    [JsonPropertyName("response_type")]
    public string ResponseType
    {
        get
        {
            return Type switch
            {
                Responses.ResponseType.Channel => "in_channel",
                Responses.ResponseType.User => "ephemeral",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}

public enum ResponseType
{
    Channel,
    User
}
