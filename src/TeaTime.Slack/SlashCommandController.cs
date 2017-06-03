namespace TeaTime.Slack
{
    using System;
    using System.Collections.Generic;
    using Common.Models;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Route("slack")]
    public class SlackController : Controller
    {
        [HttpPost]
        [Route("slash")]
        public ActionResult SlashCommandHook(SlashCommand slashCommand)
        {

            var options = new List<Option>
            {
                new Option
                {
                    Id = Guid.NewGuid(),
                    Text = "Option 1"
                }
            };

            return Ok(new SlashCommandResponse
            {
                InChannel = true,
                Text = "Hell world!",
                Attachments = AttachmentBuilder.BuildOptions(options)
            });
        }

        [HttpPost]
        [Route("interactive-messages")]
        public ActionResult InteractiveMessageHook()
        {
            return Ok();
        }
    }


}
