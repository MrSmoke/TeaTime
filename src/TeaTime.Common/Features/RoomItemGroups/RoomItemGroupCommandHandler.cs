namespace TeaTime.Common.Features.RoomItemGroups
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using Commands;
    using Common.Models.Data;
    using MediatR;

    public class RoomItemGroupCommandHandler :
        IRequestHandler<CreateRoomItemGroupCommand>,
        IRequestHandler<DeleteRoomItemGroupCommand>
    {
        private readonly IOptionsRepository _optionsRepository;
        private readonly ISystemClock _clock;

        public RoomItemGroupCommandHandler(IOptionsRepository optionsRepository, ISystemClock clock)
        {
            _optionsRepository = optionsRepository;
            _clock = clock;
        }

        public Task Handle(CreateRoomItemGroupCommand request, CancellationToken cancellationToken)
        {
            var roomGroup = new RoomItemGroup
            {
                Id = request.Id,
                Name = request.Name,
                CreatedBy = request.UserId,
                CreatedDate = _clock.UtcNow(),
                RoomId = request.RoomId
            };

            return _optionsRepository.CreateGroupAsync(roomGroup);

            //todo: event
        }

        public Task Handle(DeleteRoomItemGroupCommand request, CancellationToken cancellationToken)
        {
            return _optionsRepository.DeleteGroupAsync(request.GroupId);
        }
    }
}
