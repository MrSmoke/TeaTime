namespace TeaTime.Slack.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class SlashCommandResponse
    {
        public string Text { get; set; }
        public  IEnumerable<Attachment> Attachments { get; set; }

        public bool InChannel { get; set; }

        [JsonProperty("response_type")]
        public string ResponseType => InChannel ? "in_channel" : null;
    }
}
