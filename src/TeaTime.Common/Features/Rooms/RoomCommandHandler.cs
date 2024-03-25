namespace TeaTime.Common.Features.Rooms
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using Commands;
    using Events;
    using MediatR;
    using Models.Data;

    public class RoomCommandHandler(
        IIdGenerator<string> stringIdGenerator,
        IRoomRepository roomRepository,
        IEventPublisher eventPublisher,
        TimeProvider clock)
        : IRequestHandler<CreateRoomCommand>
    {
        public async Task Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            var room = new Room
            {
                Id = request.Id,
                RoomCode = await stringIdGenerator.GenerateAsync(),
                Name = request.Name,
                CreatedBy = request.UserId,
                CreatedDate = clock.GetUtcNow()
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
    }
}
