namespace TeaTime.Slack
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CommandRouter;
    using CommandRouter.Exceptions;
    using CommandRouter.Integration.AspNetCore;
    using Microsoft.AspNetCore.Mvc;
    using Models.Requests;
    using Models.Responses;

    [Route("slack")]
    public class SlackController : Controller
    {
        private readonly ICommandRunner _commandRunner;

        public SlackController(ICommandRunner commandRunner)
        {
            _commandRunner = commandRunner;
        }

        [HttpPost]
        [Route("slash")]
        public async Task<IActionResult> SlashCommandHook(SlashCommand slashCommand)
        {
            try
            {
                var result = await _commandRunner.RunAsync(slashCommand.Text, new Dictionary<string, object>
                {
                    {"SLASHCOMMAND", slashCommand}
                }).ConfigureAwait(false);

                Response.ContentType = "application/json";
                return new CommandRouterResult(result);
            }
            catch (CommandNotFoundException)
            {
                return Ok(new SlashCommandResponse("Unknown command", ResponseType.User));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Ok(new SlashCommandResponse("Failed to run command", ResponseType.User));
            }
        }

        [HttpPost]
        [Route("interactive-messages")]
        public ActionResult InteractiveMessageHook(MessageAction messageAction)
        {
            throw new NotImplementedException();
        }
    }
}
