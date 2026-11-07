namespace TeaTime.Common.Models.Data;

public record RoomEntity : BaseNamedEntity
{
    /// <summary>
    /// The id of the user who created this room
    /// </summary>
    public required long CreatedBy { get; init; }
}
