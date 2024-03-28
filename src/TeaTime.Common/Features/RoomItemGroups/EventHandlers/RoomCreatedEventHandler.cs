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
                Id: await _idGenerator.GenerateAsync(),
                RoomId: notification.Id,
                UserId: notification.CreatedBy,
                Name: "tea");

            await _mediator.Send(command, CancellationToken.None);

            //create some options
            foreach (var name in DefaultItemNames)
            {
                var optionCommand = new CreateOptionCommand(
                    Id: await _idGenerator.GenerateAsync(),
                    UserId: notification.CreatedBy,
                    GroupId: command.Id,
                    Name: name);

                await _mediator.Send(optionCommand, CancellationToken.None);
            }
        }
    }
}
