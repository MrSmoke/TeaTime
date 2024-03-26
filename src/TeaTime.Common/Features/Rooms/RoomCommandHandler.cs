namespace TeaTime.Common.Features.Rooms
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using Commands;
    using Events;
    using MediatR;
    using Models.Data;

    public class RoomCommandHandler(
        IRoomRepository roomRepository,
        IEventPublisher eventPublisher,
        TimeProvider clock)
        : IRequestHandler<CreateRoomCommand>, IRequestHandler<SetRoomCodeCommand>
    {
        public async Task Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            var room = new Room
            {
                Id = request.Id,
                Name = request.Name,
                CreatedBy = request.UserId,
                CreatedDate = clock.GetUtcNow(),

                // Room codes need to be generated separately
                RoomCode = null,
            };

            await roomRepository.CreateAsync(room);

            var evt = new RoomCreatedEvent
            (
                Id: request.Id,
                Name: request.Name,
                CreatedBy: request.UserId,
                CreateDefaultItemGroup: request.CreateDefaultItemGroup
            );

            await eventPublisher.PublishAsync(evt);
        }

        public async Task Handle(SetRoomCodeCommand request, CancellationToken cancellationToken)
        {
            // todo: exception
            if (request.RoomCode.Length > 24)
                throw new Exception("roomCode too long");

            var room = await roomRepository.GetAsync(request.RoomId);

            // todo: exception
            if (room is null)
                throw new Exception("unknown room");

            await roomRepository.UpdateAsync(room with
            {
                RoomCode = request.RoomCode
            });
        }
    }
}
