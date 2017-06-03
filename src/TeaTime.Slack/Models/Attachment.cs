namespace TeaTime.Slack.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

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

        [JsonProperty("callback_id")]
        public string CallBackId { get; set; }

        public string Color { get; set; }

        [JsonProperty("attachment_type")]
        public string AttachmentType { get; set; } = "default";

        public List<Action> Actions { get; }
    }
}
