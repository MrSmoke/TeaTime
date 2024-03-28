namespace TeaTime.Common.Features.RoomItemGroups;

using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Extensions;
using MediatR;
using Models;
using Queries;

public class RoomItemGroupQueryHandler :
    IRequestHandler<GetRoomItemGroupByNameQuery, RoomItemGroupModel?>,
    IRequestHandler<GetRoomItemGroupQuery, RoomItemGroupModel?>
{
    private readonly IOptionsRepository _optionsRepository;

    public RoomItemGroupQueryHandler(IOptionsRepository optionsRepository)
    {
        _optionsRepository = optionsRepository;
    }

    public async Task<RoomItemGroupModel?> Handle(GetRoomItemGroupByNameQuery request,
        CancellationToken cancellationToken)
    {
        var group = await _optionsRepository.GetGroupByNameAsync(request.RoomId, request.Name);
        if (group == null)
            return null;

        var model = new RoomItemGroupModel
        {
            Id = group.Id,
            Name = group.Name,
            CreatedBy = group.CreatedBy,
            CreatedDate = group.CreatedDate,
            RoomId = group.RoomId,
            Options = (await _optionsRepository.GetOptionsByGroupIdAsync(group.Id)).AsReadOnlyList()
        };

        return model;
    }

    public async Task<RoomItemGroupModel?> Handle(GetRoomItemGroupQuery request, CancellationToken cancellationToken)
    {
        var group = await _optionsRepository.GetGroupAsync(request.GroupId);
        if (group == null)
            return null;

        var model = new RoomItemGroupModel
        {
            Id = group.Id,
            Name = group.Name,
            CreatedBy = group.CreatedBy,
            CreatedDate = group.CreatedDate,
            RoomId = group.RoomId,
            Options = (await _optionsRepository.GetOptionsByGroupIdAsync(group.Id)).AsReadOnlyList()
        };

        return model;
    }
}
