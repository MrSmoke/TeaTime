namespace TeaTime.Slack.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class Attachment
    {
        internal Attachment()
        {
            Actions = new List<Action>();
        }

        /// <summary>
        /// This is the main text in a message attachment
        /// </summary>
        /// <remarks>Optional</remarks>
        public string Text { get; set; }

        /// <summary>
        /// A plain-text summary of the attachment
        /// </summary>
        public string Fallback { get; set; }

        [JsonPropertyName("callback_id")]
        public string CallBackId { get; set; }

        public string Color { get; set; }

        [JsonPropertyName("attachment_type")]
        public string AttachmentType { get; set; } = "default";

        public List<Action> Actions { get; }
    }
}
