namespace TeaTime.Common.Features.RoomItemGroups.EventHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Commands;
    using MediatR;
    using Options.Commands;
    using Rooms.Events;

    public class RoomCreatedEventHandler : INotificationHandler<RoomCreatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly IIdGenerator<long> _idGenerator;

        private static readonly string[] DefaultItemNames = {"Earl Grey", "English Breakfast"};

        public RoomCreatedEventHandler(IMediator mediator, IIdGenerator<long> idGenerator)
        {
            _mediator = mediator;
            _idGenerator = idGenerator;
        }

        public async Task Handle(RoomCreatedEvent notification, CancellationToken cancellationToken)
        {
            if (!notification.CreateDefaultItemGroup)
                return;

            var command = new CreateRoomItemGroupCommand(
                id: await _idGenerator.GenerateAsync(),
                roomId: notification.Id,
                userId: notification.CreatedBy,
                name: "tea");

            await _mediator.Send(command, CancellationToken.None);

            //create some options
            foreach (var name in DefaultItemNames)
            {
                var optionCommand = new CreateOptionCommand(
                    id: await _idGenerator.GenerateAsync(),
                    userId: notification.CreatedBy,
                    groupId: command.Id,
                    name: name);

                await _mediator.Send(optionCommand, CancellationToken.None);
            }
        }
    }
}
