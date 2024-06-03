namespace TeaTime.Common.Features.Rooms
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using Commands;
    using Events;
    using MediatR;
    using Models.Data;

    public class RoomCommandHandler : IRequestHandler<CreateRoomCommand>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISystemClock _clock;

        public RoomCommandHandler(IRoomRepository roomRepository,
            IEventPublisher eventPublisher,
            ISystemClock clock)
        {
            _roomRepository = roomRepository;
            _eventPublisher = eventPublisher;
            _clock = clock;
        }

        public async Task Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            var room = new Room
            {
                Id = request.Id,
                Name = request.Name,
                CreatedBy = request.UserId,
                CreatedDate = _clock.UtcNow()
            };

            await _roomRepository.CreateAsync(room);

            var evt = new RoomCreatedEvent
            (
                Id: request.Id,
                Name: request.Name,
                CreatedBy: request.UserId,
                CreateDefaultItemGroup: request.CreateDefaultItemGroup
            );

            await _eventPublisher.PublishAsync(evt);
        }
    }
}
