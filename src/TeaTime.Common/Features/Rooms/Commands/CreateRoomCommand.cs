namespace TeaTime.Common.Features.Rooms.Commands;

using Abstractions;

/// <summary>
/// Creates a room
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="CurrentUserId">The user id of the user who created the room</param>
/// <param name="CreateDefaultItemGroup">Set to true to create the default item groups for this room</param>
public record CreateRoomCommand(long Id, string Name, long CurrentUserId, bool CreateDefaultItemGroup = true) : IUserCommand;
