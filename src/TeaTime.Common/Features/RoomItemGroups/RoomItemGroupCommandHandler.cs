namespace TeaTime.Common.Features.RoomItemGroups
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using AutoMapper;
    using Commands;
    using Common.Models.Data;
    using MediatR;

    public class RoomItemGroupCommandHandler :
        IRequestHandler<CreateRoomItemGroupCommand>,
        IRequestHandler<DeleteRoomItemGroupCommand>
    {
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;
        private readonly IOptionsRepository _optionsRepository;
        private readonly ISystemClock _clock;

        public RoomItemGroupCommandHandler(IMapper mapper, IEventPublisher eventPublisher, IOptionsRepository optionsRepository, ISystemClock clock)
        {
            _mapper = mapper;
            _eventPublisher = eventPublisher;
            _optionsRepository = optionsRepository;
            _clock = clock;
        }

        public async Task<Unit> Handle(CreateRoomItemGroupCommand request, CancellationToken cancellationToken)
        {
            var roomGroup = _mapper.Map<CreateRoomItemGroupCommand, RoomItemGroup>(request);

            roomGroup.CreatedDate = _clock.UtcNow();

            await _optionsRepository.CreateGroupAsync(roomGroup);

            //todo: event

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteRoomItemGroupCommand request, CancellationToken cancellationToken)
        {
            await _optionsRepository.DeleteGroupAsync(request.GroupId);

            return Unit.Value;
        }
    }
}
