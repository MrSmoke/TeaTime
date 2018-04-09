namespace TeaTime.Slack.Models.Requests.InteractiveMessages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class MessageRequestPayload : IVerifiableRequest
    {
        public string Type { get; set; }
        public List<MessageAction> Actions { get; set; }
        public MessageTeam Team { get; set; }
        public MessageChannel Channel { get; set; }
        public MessageUser User { get; set; }
        public string Token { get; set; }

        [JsonProperty("response_url")]
        public string ResponseUrl { get; set; }
    }
}