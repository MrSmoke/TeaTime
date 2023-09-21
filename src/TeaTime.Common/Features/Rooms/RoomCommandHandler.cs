namespace TeaTime.Common.Features.Rooms
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using AutoMapper;
    using Commands;
    using Events;
    using MediatR;
    using Models.Data;

    public class RoomCommandHandler : IRequestHandler<CreateRoomCommand>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISystemClock _clock;

        public RoomCommandHandler(IRoomRepository roomRepository, IMapper mapper, IEventPublisher eventPublisher, ISystemClock clock)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
            _clock = clock;
        }

        public async Task<Unit> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            var room = _mapper.Map<CreateRoomCommand, Room>(request);

            room.CreatedDate = _clock.UtcNow();

            await _roomRepository.CreateAsync(room);

            var evt = _mapper.Map<CreateRoomCommand, RoomCreatedEvent>(request);

            await _eventPublisher.PublishAsync(evt);

            return Unit.Value;
        }
    }
}
