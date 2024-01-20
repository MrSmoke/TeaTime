namespace TeaTime.Common.Features.RoomItemGroups.Models;

using System.Collections.Generic;
using Common.Models.Data;

public record RoomItemGroupModel : RoomItemGroup
{
    public IReadOnlyList<Option> Options { get; init; } = new List<Option>();
}
