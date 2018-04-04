namespace TeaTime.Slack.Models.Requests
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    public class MessageRequest
    {
        [FromForm(Name = "payload")]
        public string PayloadJson { get; set; }
    }

    public class MessageRequestPayload
    {
        public string Type { get; set; }
        public List<MessageAction> Actions { get; set; }
        public MessageTeam Team { get; set; }
        public MessageChannel Channel { get; set; }
        public MessageUser User { get; set; }
    }

    public class MessageAction
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class MessageTeam
    {
        public string Id { get; set; }
        public string Domain { get; set; }
    }

    public class MessageChannel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class MessageUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
