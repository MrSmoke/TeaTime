namespace TeaTime.Slack
{
    using System;
    using System.Collections.Generic;
    using Common.Models;
    using Microsoft.AspNetCore.Mvc;
    using Models.Requests;
    using Models.Responses;

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
                    Text = ":fire:"
                },
                new Option
                {
                    Id = Guid.NewGuid(),
                    Text = ":fire: 2"
                },
                new Option
                {
                    Id = Guid.NewGuid(),
                    Text = ":fire: 3"
                },
                new Option
                {
                    Id = Guid.NewGuid(),
                    Text = ":fire: 4"
                },
                new Option
                {
                    Id = Guid.NewGuid(),
                    Text = ":fire: 5"
                },
                new Option
                {
                    Id = Guid.NewGuid(),
                    Text = ":fire: 6"
                },
            };

            return Ok(new SlashCommandResponse
            {
                Type = ResponseType.Channel,
                Text = "Hell world!",
                Attachments = AttachmentBuilder.BuildOptions(options)
            });
        }

        [HttpPost]
        [Route("interactive-messages")]
        public ActionResult InteractiveMessageHook(MessageAction messageAction)
        {
            return Ok();
        }
    }


}
