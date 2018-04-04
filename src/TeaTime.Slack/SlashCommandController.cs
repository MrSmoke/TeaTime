namespace TeaTime.Slack
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CommandRouter;
    using CommandRouter.Exceptions;
    using CommandRouter.Integration.AspNetCore;
    using Common.Abstractions;
    using Common.Features.Options;
    using Common.Features.Rooms.Queries;
    using Common.Features.Runs.Commands;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Requests;
    using Models.Responses;
    using Newtonsoft.Json;

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
        public async Task<IActionResult> InteractiveMessageHook(MessageRequest request)
        {
            var message = JsonConvert.DeserializeObject<MessageRequestPayload>(request.PayloadJson);

            var user = await _slackService.GetOrCreateUser(message.User.Id, message.User.Name).ConfigureAwait(false);
            var room = await _slackService.GetOrCreateRoom(message.Channel.Id, message.Channel.Name, user.Id).ConfigureAwait(false);

            var firstAction = message.Actions.First();

            var run = await _mediator.Send(new GetCurrentRunQuery(room.Id, user.Id)).ConfigureAwait(false);
            if (run == null)
                return Ok($"Please start first", ResponseType.User);

            var optionId = long.Parse(firstAction.Value);

            var option = await _mediator.Send(new GetOptionQuery(optionId)).ConfigureAwait(false);
            if (option == null)
                return Ok("Unknown option", ResponseType.User);

            var command = new JoinRunCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                runId: run.Id,
                userId: user.Id,
                optionId: option.Id);

            await _mediator.Send(command).ConfigureAwait(false);

            return Ok($"{user.DisplayName} has joined this round", ResponseType.Channel);
        }

        private IActionResult Ok(string message, ResponseType responseType)
        {
            return Ok(new SlashCommandResponse(message, responseType));
        }
    }
}
