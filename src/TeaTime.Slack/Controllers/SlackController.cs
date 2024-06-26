namespace TeaTime.Slack.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CommandRouter;
    using CommandRouter.Exceptions;
    using CommandRouter.Integration.AspNetCore;
    using Common.Exceptions;
    using Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.InteractiveMessages;
    using Models.SlashCommands;
    using Resources;
    using Services;

    [Route("slack")]
    public class SlackController : ControllerBase
    {
        private readonly ICommandRunner _commandRunner;
        private readonly ISlackService _slackService;
        private readonly ILogger<SlackController> _logger;

        public SlackController(ICommandRunner commandRunner,
            ISlackService slackService,
            ILogger<SlackController> logger)
        {
            _commandRunner = commandRunner;
            _slackService = slackService;
            _logger = logger;
        }

        [SlackVerifyRequest]
        [HttpPost("slash")]
        public async Task<IActionResult> SlashCommandHook(SlashCommand slashCommand)
        {
            if (slashCommand == null)
            {
                _logger.LogError("Null slash command");
                return Ok(ErrorStrings.General(), ResponseType.User);
            }

            if (string.IsNullOrWhiteSpace(slashCommand.Text))
            {
                _logger.LogWarning("Slash command contains no text");
                return Ok(ErrorStrings.General(), ResponseType.User);
            }

            _logger.LogInformation("Received slash command {Text} {@Command}", slashCommand.Text, slashCommand);

            try
            {
                var result = await _commandRunner.RunAsync(slashCommand.Text, new Dictionary<string, object>
                {
                    {Constants.CommandContextKeys.SlashCommand, slashCommand}
                });

                Response.ContentType = "application/json";
                return new CommandRouterResult(result);
            }
            catch (CommandNotFoundException)
            {
                return Ok(ErrorStrings.CommandUnknown(slashCommand.Text), ResponseType.User);
            }
            catch (RunStartException e)
            {
                switch (e.Reason)
                {
                    case RunStartException.RunStartExceptionReason.ExistingActiveRun:
                        return Ok(ErrorStrings.StartRun_RunAlreadyStarted(), ResponseType.User);

                    case RunStartException.RunStartExceptionReason.Unspecified:
                    default:
                        break;
                }

                _logger.LogError(e, "Failed to start run");
            }
            catch (RunEndException e)
            {
                switch (e.Reason)
                {
                    case RunEndException.RunEndExceptionReason.NoActiveRun:
                        return Ok(ErrorStrings.EndRun_RunNotStarted(), ResponseType.User);

                    case RunEndException.RunEndExceptionReason.NoOrders:
                        return Ok(ErrorStrings.EndRun_NoOrders(), ResponseType.User);

                    case RunEndException.RunEndExceptionReason.NotJoined:
                        return Ok(ErrorStrings.EndRun_NotJoined(), ResponseType.User);

                    case RunEndException.RunEndExceptionReason.Unspecified:
                    default:
                        break;
                }

                _logger.LogError(e, "Failed to end run");
            }
            catch (PermissionException e)
            {
                return Ok(e.Message, ResponseType.User);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to run command");
            }

            return Ok(ErrorStrings.CommandFailed(), ResponseType.User);
        }

        [SlackVerifyRequest]
        [HttpPost("interactive-messages")]
        public async Task<IActionResult> InteractiveMessageHook(MessageRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Null interactive message request");
                return Ok(ErrorStrings.General(), ResponseType.User);
            }

            if (string.IsNullOrWhiteSpace(request.PayloadJson))
            {
                _logger.LogError("PayloadJson is empty");
                return Ok(ErrorStrings.General(), ResponseType.User);
            }

            var message = SlackJsonSerializer.Deserialize<MessageRequestPayload>(request.PayloadJson);
            if (message == null)
            {
                _logger.LogError("Failed to deserialize interactive message request");
                return Ok(ErrorStrings.General(), ResponseType.User);
            }

            _logger.LogInformation("Received interactive message {@Request}", message);

            try
            {
                await _slackService.JoinRunAsync(message);

                return Ok();
            }
            catch (SlackTeaTimeException e)
            {
                return Ok(e.Message, ResponseType.User);
            }
            catch (PermissionException e)
            {
                return Ok(e.Message, ResponseType.User);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to join run");
                return Ok(ErrorStrings.General(), ResponseType.User);
            }
        }

        private OkObjectResult Ok(string message, ResponseType responseType)
        {
            return Ok(new SlashCommandResponse(message, responseType));
        }
    }
}
