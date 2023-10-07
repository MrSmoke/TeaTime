namespace TeaTime.Slack.Models.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class SlashCommandResponse
    {
        public SlashCommandResponse()
        {
            Type = Responses.ResponseType.Channel;
        }

        public SlashCommandResponse(string? text, ResponseType responseType)
        {
            Type = responseType;
            Text = text;
        }

        public string? Text { get; set; }

        [JsonPropertyName("replace_original")]
        public bool ReplaceOriginal { get; set; }

        [JsonIgnore]
        public ResponseType Type { get; set; }

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
}
