#nullable disable
namespace TeaTime.Slack.Models.Requests.InteractiveMessages
{
    using Microsoft.AspNetCore.Mvc;

    public class MessageRequest
    {
        [FromForm(Name = "payload")]
        public string PayloadJson { get; set; }
    }
}
