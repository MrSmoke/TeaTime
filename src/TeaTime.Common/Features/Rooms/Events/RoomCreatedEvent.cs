namespace TeaTime.Common.Features.Rooms.Events;

using Abstractions;

/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="CreatedBy">The id of the user who created the room</param>
/// <param name="CreateDefaultItemGroup">Set to true to create the default item groups for this room</param>
public record RoomCreatedEvent
(
    long Id,
    string Name,
    long CreatedBy,
    bool CreateDefaultItemGroup
) : BaseEvent;
