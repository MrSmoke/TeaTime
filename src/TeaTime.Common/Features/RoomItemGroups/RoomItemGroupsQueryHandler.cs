namespace TeaTime.Common.Features.RoomItemGroups;

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using MediatR;
using Models;
using Queries;

public class RoomItemGroupQueryHandler(IOptionsRepository optionsRepository) :
    IRequestHandler<GetRoomItemGroupByNameQuery, RoomItemGroupModel?>,
    IRequestHandler<GetRoomItemGroupQuery, RoomItemGroupModel?>
{
    // StringComparer which handles numeric ordering so "10" is after "9" not "1".
    // This ensures all options are returned in the same order every time.
    private static readonly StringComparer OptionNameComparer = StringComparer.Create(
        CultureInfo.InvariantCulture,
        CompareOptions.NumericOrdering | CompareOptions.IgnoreCase
    );

    public async Task<RoomItemGroupModel?> Handle(GetRoomItemGroupByNameQuery request,
        CancellationToken cancellationToken)
    {
        var group = await optionsRepository.GetGroupByNameAsync(request.RoomId, request.Name, cancellationToken);
        if (group == null)
            return null;

        var options = await optionsRepository.GetOptionsByGroupIdAsync(group.Id, cancellationToken);

        var model = new RoomItemGroupModel
        {
            Id = group.Id,
            Name = group.Name,
            CreatedBy = group.CreatedBy,
            CreatedDate = group.CreatedDate,
            RoomId = group.RoomId,
            Options = [.. options.OrderBy(o => o.Name, OptionNameComparer)]
        };

        return model;
    }

    public async Task<RoomItemGroupModel?> Handle(GetRoomItemGroupQuery request, CancellationToken cancellationToken)
    {
        var group = await optionsRepository.GetGroupAsync(request.GroupId, cancellationToken);
        if (group == null)
            return null;

        var options = await optionsRepository.GetOptionsByGroupIdAsync(group.Id, cancellationToken);

        var model = new RoomItemGroupModel
        {
            Id = group.Id,
            Name = group.Name,
            CreatedBy = group.CreatedBy,
            CreatedDate = group.CreatedDate,
            RoomId = group.RoomId,
            Options = [.. options.OrderBy(o => o.Name, OptionNameComparer)]
        };

        return model;
    }
}
