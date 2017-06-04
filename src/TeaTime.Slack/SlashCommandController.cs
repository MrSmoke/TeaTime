namespace TeaTime.Slack
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CommandRouter;
    using CommandRouter.Exceptions;
    using Common.Services;
    using Microsoft.AspNetCore.Mvc;
    using Models.Requests;
    using Models.Responses;

    [Route("slack")]
    public class SlackController : Controller
    {
        private readonly ICommandRunner _commandRunner;
        private readonly SlackCommand _slackService;

        public SlackController(IUserService userService, IRoomService roomService, IRunService runService, ICommandRunner commandRunner)
        {
            _commandRunner = commandRunner;
            _slackService = new SlackCommand(userService, runService, roomService);
        }

        [HttpPost]
        [Route("slash")]
        public async Task<IActionResult> SlashCommandHook(SlashCommand slashCommand)
        {
            _slackService.Command = slashCommand;

            try
            {
                var result = await _commandRunner.RunAsync(slashCommand.Text, new Dictionary<string, object>
                {
                    {"SLASHCOMMAND", slashCommand}
                }).ConfigureAwait(false);

                return new CommandRunnerActionResult(result);
            }
            catch (CommandNotFoundException)
            {
                return Ok(new SlashCommandResponse("Unknown command", ResponseType.User));
            }
            catch (Exception e)
            {
                return Ok(new SlashCommandResponse("Failed to run command", ResponseType.User));
            }
        }

        [HttpPost]
        [Route("interactive-messages")]
        public ActionResult InteractiveMessageHook(MessageAction messageAction)
        {
            return Ok();
        }
    }


}
