﻿namespace TeaTime.Common.Features.RoomItemGroups
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
        private readonly IOptionsRepository _optionsRepository;
        private readonly ISystemClock _clock;

        public RoomItemGroupCommandHandler(IMapper mapper, IOptionsRepository optionsRepository, ISystemClock clock)
        {
            _mapper = mapper;
            _optionsRepository = optionsRepository;
            _clock = clock;
        }

        public Task Handle(CreateRoomItemGroupCommand request, CancellationToken cancellationToken)
        {
            var roomGroup = _mapper.Map<CreateRoomItemGroupCommand, RoomItemGroup>(request);

            roomGroup.CreatedDate = _clock.UtcNow();

            return _optionsRepository.CreateGroupAsync(roomGroup);

            //todo: event
        }

        public Task Handle(DeleteRoomItemGroupCommand request, CancellationToken cancellationToken)
        {
            return _optionsRepository.DeleteGroupAsync(request.GroupId);
        }
    }
}
