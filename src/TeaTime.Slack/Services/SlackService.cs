namespace TeaTime.Slack.Services
{
    using System;
    using System.Threading.Tasks;
    using Common.Abstractions;
    using Common.Features.Links.Commands;
    using Common.Features.Links.Queries;
    using Common.Features.Rooms.Commands;
    using Common.Features.Rooms.Queries;
    using Common.Features.Users.Commands;
    using Common.Features.Users.Queries;
    using Common.Models;
    using Common.Models.Data;
    using MediatR;
    using Models.Responses;

    public class SlackService : ISlackService
    {
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;

        public SlackService(IMediator mediator, IIdGenerator<long> idGenerator)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
        }

        public async Task<User> GetOrCreateUser(string userId, string name)
        {
            var id = await _mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.User, userId)).ConfigureAwait(false);
            if (id > 0)
                return await _mediator.Send(new GetUserQuery(id)).ConfigureAwait(false);

            //we need to create a new user
            var command = new CreateUserCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                username: "slack_" + userId,
                displayName: name
            );
            await _mediator.Send(command).ConfigureAwait(false);

            //add link
            await _mediator.Send(new CreateLinkCommand(command.Id, LinkType.User, userId)).ConfigureAwait(false);

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
            var roomId = await _mediator.Send(new GetObjectIdByLinkValueQuery(LinkType.Room, channelId)).ConfigureAwait(false);
            if (roomId > 0)
                return await _mediator.Send(new GetRoomQuery(roomId)).ConfigureAwait(false);

            var command = new CreateRoomCommand(
                id: await _idGenerator.GenerateAsync().ConfigureAwait(false),
                name: channelName,
                userId: userId
            );

            await _mediator.Send(command).ConfigureAwait(false);

            //add link
            await _mediator.Send(new CreateLinkCommand(command.Id, LinkType.Room, channelId)).ConfigureAwait(false);

            return new Room
            {
                Id = command.Id,
                Name = command.Name,
                CreatedDate = DateTimeOffset.MinValue //dunno this value *shrug*
            };
        }

        private static SlashCommandResponse Response(string message, ResponseType responseType)
        {
            return new SlashCommandResponse(message, responseType);
        }
    }
}
