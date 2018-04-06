namespace TeaTime.Slack.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CommandRouter;
    using CommandRouter.Exceptions;
    using CommandRouter.Integration.AspNetCore;
    using Common.Abstractions;
    using Common.Exceptions;
    using Common.Features.Options;
    using Common.Features.Rooms.Queries;
    using Common.Features.Runs.Commands;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Models.Requests;
    using Models.Responses;
    using Newtonsoft.Json;
    using Resources;
    using Services;

    [Route("slack")]
    public class SlackController : Controller
    {
        private readonly ICommandRunner _commandRunner;
        private readonly ISlackService _slackService;
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;

        public SlackController(ICommandRunner commandRunner, ISlackService slackService, IMediator mediator, IIdGenerator<long> idGenerator)
        {
            _commandRunner = commandRunner;
            _slackService = slackService;
            _mediator = mediator;
            _idGenerator = idGenerator;
        }

        [HttpPost]
        [Route("slash")]
        public async Task<IActionResult> SlashCommandHook(SlashCommand slashCommand)
        {
            try
            {
                var result = await _commandRunner.RunAsync(slashCommand.Text, new Dictionary<string, object>
                {
                    {Constants.SlashCommand, slashCommand}
                }).ConfigureAwait(false);

                Response.ContentType = "application/json";
                return new CommandRouterResult(result);
            }
            catch (CommandNotFoundException)
            {
                return Ok(new SlashCommandResponse(ErrorStrings.CommandUnknown(slashCommand.Text), ResponseType.User));
            }
            catch (RunStartException e)
            {
                switch (e.Reason)
                {
                    case RunStartException.RunStartExceptionReason.ExistingActiveRun:
                        return Ok(new SlashCommandResponse(ErrorStrings.StartRun_RunAlreadyStarted(), ResponseType.User));

                    case RunStartException.RunStartExceptionReason.Unspecified:
                    default:
                        break;
                }
            }
            catch (RunEndException e)
            {
                switch (e.Reason)
                {
                    case RunEndException.RunEndExceptionReason.NoActiveRun:
                        return Ok(new SlashCommandResponse(ErrorStrings.EndRun_RunNotStarted(), ResponseType.User));

                    case RunEndException.RunEndExceptionReason.NoOrders:
                        return Ok(new SlashCommandResponse(ErrorStrings.EndRun_NoOrders(), ResponseType.User));

                    case RunEndException.RunEndExceptionReason.NotJoined:
                        return Ok(new SlashCommandResponse(ErrorStrings.EndRun_NotJoined(), ResponseType.User));

                    case RunEndException.RunEndExceptionReason.Unspecified:
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Ok(new SlashCommandResponse(ErrorStrings.CommandFailed(), ResponseType.User));
        }

        [HttpPost]
        [Route("interactive-messages")]
        public async Task<IActionResult> InteractiveMessageHook(MessageRequest request)
        {
            var message = JsonConvert.DeserializeObject<MessageRequestPayload>(request.PayloadJson);

            var user = await _slackService.GetOrCreateUser(message.User.Id, message.User.Name).ConfigureAwait(false);
            var room = await _slackService.GetOrCreateRoom(message.Channel.Id, message.Channel.Name, user.Id).ConfigureAwait(false);

            var firstAction = message.Actions.First();

            var run = await _mediator.Send(new GetCurrentRunQuery(room.Id, user.Id)).ConfigureAwait(false);
            if (run == null)
                return Ok(ErrorStrings.JoinRun_RunNotStarted(), ResponseType.User);

            var optionId = long.Parse(firstAction.Value);

            var option = await _mediator.Send(new GetOptionQuery(optionId)).ConfigureAwait(false);
            if (option == null)
                return Ok(ErrorStrings.OptionUnknown(), ResponseType.User);

            var command = new JoinRunCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                runId: run.Id,
                userId: user.Id,
                optionId: option.Id);

            await _mediator.Send(command).ConfigureAwait(false);

            return Ok(ResponseStrings.RunUserJoined(message.User.Id), ResponseType.Channel);
        }

        private IActionResult Ok(string message, ResponseType responseType)
        {
            return Ok(new SlashCommandResponse(message, responseType));
        }
    }
}
