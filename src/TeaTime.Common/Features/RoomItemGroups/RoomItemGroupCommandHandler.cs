namespace TeaTime.Common.Features.RoomItemGroups;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Commands;
using Common.Models.Data;
using MediatR;

public class RoomItemGroupCommandHandler(IOptionsRepository optionsRepository, TimeProvider clock) :
    IRequestHandler<CreateRoomItemGroupCommand>,
    IRequestHandler<DeleteRoomItemGroupCommand>
{
    public Task Handle(CreateRoomItemGroupCommand request, CancellationToken cancellationToken)
    {
        var roomGroup = new RoomItemGroupEntity
        {
            Id = request.Id,
            Name = request.Name,
            CreatedBy = request.UserId,
            CreatedDate = clock.GetUtcNow(),
            RoomId = request.RoomId
        };

        return optionsRepository.CreateGroupAsync(roomGroup, cancellationToken);

        //todo: event
    }

    public Task Handle(DeleteRoomItemGroupCommand request, CancellationToken cancellationToken)
    {
        return optionsRepository.DeleteGroupAsync(request.GroupId, cancellationToken);
    }
}
