namespace TeaTime.Common.Features.RoomItemGroups.Models;

using System.Collections.Generic;
using Common.Models.Data;

public record RoomItemGroupModel : RoomItemGroupEntity
{
    public required IReadOnlyList<OptionEntity> Options { get; init; }
}
