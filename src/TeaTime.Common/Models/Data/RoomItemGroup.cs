namespace TeaTime.Common.Models.Data;

public record RoomItemGroup : BaseNamedDataObject
{
    public required long RoomId { get; init; }

    /// <summary>
    /// The id of the user who created this item
    /// </summary>
    public required long CreatedBy { get; init; }
}
