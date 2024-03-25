namespace TeaTime.Common.Models.Data;

public record Room : BaseNamedDataObject
{
    /// <summary>
    /// The id of the user who created this room
    /// </summary>
    public required long CreatedBy { get; init; }

    /// <summary>
    /// Unique room code that is used to access the room externally
    /// </summary>
    public required string RoomCode { get; init; }
}
