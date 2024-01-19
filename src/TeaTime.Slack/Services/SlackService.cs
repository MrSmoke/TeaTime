namespace TeaTime.Slack.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Abstractions;
    using Common.Features.Links.Commands;
    using Common.Features.Links.Queries;
    using Common.Features.Options;
    using Common.Features.Orders.Commands;
    using Common.Features.Orders.Queries;
    using Common.Features.RoomItemGroups.Queries;
    using Common.Features.Rooms.Commands;
    using Common.Features.Rooms.Queries;
    using Common.Features.Users.Commands;
    using Common.Features.Users.Queries;
    using Common.Models;
    using Common.Models.Data;
    using Exceptions;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Models;
    using Models.Requests;
    using Models.Requests.InteractiveMessages;
    using Resources;

    internal class SlackService : ISlackService
    {
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;
        private readonly ILogger<SlackService> _logger;

        public SlackService(IMediator mediator, IIdGenerator<long> idGenerator, ILogger<SlackService> logger)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
            _logger = logger;
        }

        public async Task<User> GetOrCreateUser(string userId, string name)
        {
            var id = await _mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.User, userId));
            if (id > 0)
            {
                return await _mediator.Send(new GetUserQuery(id.Value)) ??
                       throw new InvalidOperationException($"Failed to get user {id.Value}");
            }

            //we need to create a new user
            var command = new CreateUserCommand
            (
                Id: await _idGenerator.GenerateAsync(),
                Username: "slack_" + userId,
                DisplayName: name
            );
            await _mediator.Send(command);

            //add link
            await _mediator.Send(new CreateLinkCommand(command.Id, LinkType.User, userId));

            //this is not great because its not really a proper entity model, but it will do for now
            //we shouldn't query here because the command COULD eventually be eventual consistency
            return new User
            {
                Id = command.Id,
                DisplayName = command.DisplayName,
                Username = command.Username,
                CreatedDate = DateTimeOffset.MinValue //dunno this value *shrug*
            };
        }

        public async Task<Room> GetOrCreateRoom(string channelId, string channelName, long userId)
        {
            var roomId = await _mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.Room, channelId));
            if (roomId > 0)
            {
                return await _mediator.Send(new GetRoomQuery(roomId.Value)) ??
                       throw new InvalidOperationException($"Failed to get room {roomId.Value}");
            }

            var command = new CreateRoomCommand
            (
                Id: await _idGenerator.GenerateAsync(),
                Name: channelName,
                UserId: userId
            );

            await _mediator.Send(command);

            //add link
            await _mediator.Send(new CreateLinkCommand(command.Id, LinkType.Room, channelId));

            return new Room
            {
                Id = command.Id,
                Name = command.Name,
                CreatedDate = DateTimeOffset.MinValue //dunno this value *shrug*
            };
        }

        public async Task JoinRunAsync(SlashCommand slashCommand, string optionName)
        {
            var user = await GetOrCreateUser(slashCommand.UserId, slashCommand.UserName);
            var room = await GetOrCreateRoom(slashCommand.ChannelId, slashCommand.ChannelName, user.Id);

            await JoinRunAsync(user.Id, room.Id, optionName, slashCommand.ToCallbackData());
        }

        public async Task JoinRunAsync(MessageRequestPayload requestPayload)
        {
            var user = await GetOrCreateUser(requestPayload.User.Id, requestPayload.User.Name);
            var room = await GetOrCreateRoom(requestPayload.Channel.Id, requestPayload.Channel.Name, user.Id);

            var firstAction = requestPayload.Actions.First();
            var optionId = long.Parse(firstAction.Value);

            await JoinRunAsync(user.Id, room.Id, optionId, requestPayload.ToCallbackData());
        }

        private async Task JoinRunAsync(long userId, long roomId, string optionName, CallbackData callbackData)
        {
            var run = await _mediator.Send(new GetCurrentRunQuery(roomId, userId));
            if (run == null)
                throw new SlackTeaTimeException(ErrorStrings.JoinRun_RunNotStarted());

            var group = await _mediator.Send(new GetRoomItemGroupQuery(
                RoomId: run.RoomId,
                UserId: userId,
                GroupId: run.GroupId));

            if (group == null)
            {
                _logger.LogWarning("Failed to find group {GroupId} in room {RoomId} for run {RunId}",
                    run.GroupId, roomId, run.Id);

                throw new SlackTeaTimeException(ErrorStrings.General());
            }

            var option = group.Options
                .FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));

            if (option == null)
                throw new SlackTeaTimeException(ErrorStrings.OptionUnknown());

            await JoinRunInternalAsync(userId, run, option.Id, callbackData);
        }

        private async Task JoinRunAsync(long userId, long roomId, long optionId, CallbackData callbackData)
        {
            var run = await _mediator.Send(new GetCurrentRunQuery(roomId, userId));
            if (run == null)
                throw new SlackTeaTimeException(ErrorStrings.JoinRun_RunNotStarted());

            var option = await _mediator.Send(new GetOptionQuery(optionId));
            if (option == null)
                throw new SlackTeaTimeException(ErrorStrings.OptionUnknown());

            await JoinRunInternalAsync(userId, run, optionId, callbackData);
        }

        private async Task JoinRunInternalAsync(long userId, Run run, long optionId, CallbackData callbackData)
        {
            BaseCommand command;

            if (run.Ended)
                throw new SlackTeaTimeException(ErrorStrings.JoinRun_RunEnded());

            //check if we need to join or update
            var existingOrder = await _mediator.Send(new GetUserOrderQuery(run.Id, userId));
            if (existingOrder == null)
            {
                command = new CreateOrderCommand
                (
                    Id: await _idGenerator.GenerateAsync(),
                    RunId: run.Id,
                    UserId: userId,
                    OptionId: optionId
                );
            }
            else
            {
                //we need to update the existing order
                command = new UpdateOrderOptionCommand(existingOrder.Id, userId, optionId);
            }

            command.AddCallbackState(callbackData);

            await _mediator.Send(command);
        }
    }
}
