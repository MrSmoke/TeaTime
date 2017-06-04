namespace TeaTime.Slack
{
    using System.Threading.Tasks;
    using CommandRouter.Results;
    using Microsoft.AspNetCore.Mvc;

    public class CommandRunnerActionResult : IActionResult
    {
        private readonly ICommandResult _commandResult;

        public CommandRunnerActionResult(ICommandResult commandResult)
        {
            _commandResult = commandResult;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            return _commandResult.ExecuteAsync(context.HttpContext.Response.Body);
        }
    }
}
