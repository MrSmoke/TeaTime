namespace TeaTime.Slack
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Services;
    using Microsoft.AspNetCore.Mvc;
    using Models.Requests;
    using Models.Responses;

    [Route("slack")]
    public class SlackController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;
        private readonly IRunService _runService;

        public SlackController(IUserService userService, IRoomService roomService, IRunService runService)
        {
            _userService = userService;
            _roomService = roomService;
            _runService = runService;
        }

        [HttpPost]
        [Route("slash")]
        public async Task<ActionResult> SlashCommandHook(SlashCommand slashCommand)
        {
            var slackService = new SlackService(_userService, _runService, _roomService);

            slackService.Command = slashCommand;
            var result = await slackService.Start();

            return Ok(result);
        }

        [HttpPost]
        [Route("interactive-messages")]
        public ActionResult InteractiveMessageHook(MessageAction messageAction)
        {
            return Ok();
        }
    }


}
