namespace TeaTime.Common.Features.Rooms;

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
    : IRequestHandler<CreateRoomCommand>
{
    public async Task Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = new RoomEntity
        {
            Id = request.Id,
            Name = request.Name,
            CreatedBy = request.CurrentUserId,
            CreatedDate = clock.GetUtcNow()
        };

        await roomRepository.CreateAsync(room, cancellationToken);

        var evt = new RoomCreatedEvent
        (
            Id: request.Id,
            Name: request.Name,
            CreatedBy: request.CurrentUserId,
            CreateDefaultItemGroup: request.CreateDefaultItemGroup
        );

        await eventPublisher.PublishAsync(evt, cancellationToken);
    }
}
