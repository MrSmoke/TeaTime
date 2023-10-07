namespace TeaTime.Common.Features.RoomItemGroups.Models
{
    using System.Collections.Generic;
    using Common.Models.Data;

    public class RoomItemGroupModel : RoomItemGroup
    {
        public IReadOnlyList<Option> Options { get; set; } = new List<Option>();
    }
}
