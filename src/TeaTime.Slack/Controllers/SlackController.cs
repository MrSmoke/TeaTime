namespace TeaTime.Slack.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommandRouter;
using CommandRouter.Exceptions;
using CommandRouter.Integration.AspNetCore;
using Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Interactivity;
using Models.SlashCommands;
using Resources;
using Services;

[Route("slack")]
public class SlackController(
    ICommandRunner commandRunner,
    ISlackService slackService,
    ILogger<SlackController> logger)
    : ControllerBase
{
    [SlackVerifyRequest]
    [HttpPost("slash")]
    public async Task<IActionResult> SlashCommandHook(SlashCommand slashCommand)
    {
        if (slashCommand == null)
        {
            logger.LogError("Null slash command");
            return Ok(ErrorStrings.General(), ResponseType.User);
        }

        // Slack's SSL certificate check only needs an empty 200
        if (slashCommand.SslCheck == "1")
            return Ok();

        using var scope = BeginSlackScope(slashCommand.UserId, slashCommand.ChannelId, slashCommand.TeamId);

        if (string.IsNullOrWhiteSpace(slashCommand.Text))
        {
            logger.LogWarning("Slash command contains no text");
            return Ok(ErrorStrings.General(), ResponseType.User);
        }

        // Command text is user input so isn't logged
        logger.LogInformation("Received slash command from {SlackUserId} in {SlackChannelId} ({SlackTeamId})",
            slashCommand.UserId, slashCommand.ChannelId, slashCommand.TeamId);

        try
        {
            var result = await commandRunner.RunAsync(slashCommand.Text, new Dictionary<string, object>
            {
                {Constants.CommandContextKeys.SlashCommand, slashCommand}
            });

            Response.ContentType = "application/json";
            return new CommandRouterResult(result);
        }
        catch (CommandNotFoundException)
        {
            logger.LogWarning("Slash command not found");
            return Ok(ErrorStrings.CommandUnknown(slashCommand.Text), ResponseType.User);
        }
        catch (ParameterBindingException e)
        {
            // Parameter name is ours; the value is user input so it isn't logged
            logger.LogWarning("Slash command arguments invalid for parameter {ParameterName}", e.ParameterName);
            return Ok(ErrorStrings.CommandBadArguments(slashCommand.Text), ResponseType.User);
        }
        catch (RunStartException e)
        {
            logger.LogWarning("Failed to start run: {Reason}", e.Reason);

            switch (e.Reason)
            {
                case RunStartException.RunStartExceptionReason.ExistingActiveRun:
                    return Ok(ErrorStrings.StartRun_RunAlreadyStarted(), ResponseType.User);

                case RunStartException.RunStartExceptionReason.Unspecified:
                default:
                    break;
            }

            logger.LogError(e, "Failed to start run");
        }
        catch (RunEndException e)
        {
            logger.LogWarning("Failed to end run: {Reason}", e.Reason);

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

            logger.LogError(e, "Failed to end run");
        }
        catch (PermissionException e)
        {
            logger.LogWarning("Slash command denied: {Message}", e.Message);
            return Ok(e.Message, ResponseType.User);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to run command");
        }

        return Ok(ErrorStrings.CommandFailed(), ResponseType.User);
    }

    [SlackVerifyRequest]
    [HttpPost("interactions")]
    public async Task<IActionResult> InteractionHook(InteractionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.PayloadJson))
        {
            logger.LogError("Interaction payload is empty");
            return Ok();
        }

        BlockActionsPayload? payload;
        try
        {
            payload = SlackJsonSerializer.Deserialize<BlockActionsPayload>(request.PayloadJson);
        }
        catch (JsonException e)
        {
            logger.LogError(e, "Failed to deserialise interaction payload");
            return Ok();
        }

        if (payload == null)
        {
            logger.LogError("Interaction payload deserialised to null");
            return Ok();
        }

        if (payload.Type != BlockActionsPayload.PayloadType)
        {
            logger.LogWarning("Ignoring unsupported interaction type {Type}", payload.Type);
            return Ok();
        }

        using var scope = BeginSlackScope(payload.User.Id, payload.Channel?.Id, payload.Team?.Id);

        // Action IDs are ours; response_url is a credential so the payload isn't logged
        logger.LogInformation("Received block actions {ActionIds} from {SlackUserId} in {SlackChannelId} ({SlackTeamId})",
            payload.Actions.Select(a => a.ActionId), payload.User.Id, payload.Channel?.Id, payload.Team?.Id);

        await slackService.HandleBlockActionsAsync(payload);

        return Ok();
    }

    private IDisposable? BeginSlackScope(string? userId, string? channelId, string? teamId)
    {
        return logger.BeginScope(new Dictionary<string, object?>
        {
            ["SlackUserId"] = userId,
            ["SlackChannelId"] = channelId,
            ["SlackTeamId"] = teamId
        });
    }

    private ContentResult Ok(string message, ResponseType responseType)
    {
        var json = SlackJsonSerializer.Serialize(new SlashCommandResponse(message, responseType));
        return Content(json, "application/json");
    }
}
