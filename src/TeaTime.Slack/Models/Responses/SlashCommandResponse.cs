namespace TeaTime.Slack.Models.Responses
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class SlashCommandResponse
    {
        public SlashCommandResponse()
        {
            Type = Responses.ResponseType.Channel;
        }

        public SlashCommandResponse(string text, ResponseType responseType)
        {
            Type = responseType;
            Text = text;
        }

        public string Text { get; set; }

        [JsonIgnore]
        public ResponseType Type { get; set; }

        public IEnumerable<Attachment> Attachments { get; set; }

        [JsonProperty("response_type")]
        public string ResponseType
        {
            get
            {
                switch (Type)
                {
                    case Responses.ResponseType.Channel:
                        return "in_channel";
                    case Responses.ResponseType.User:
                        return "ephemeral";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public enum ResponseType
    {
        Channel,
        User
    }
}
