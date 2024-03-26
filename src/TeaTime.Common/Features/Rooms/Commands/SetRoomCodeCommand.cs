namespace TeaTime.Common.Features.Rooms.Commands;

using Abstractions;

/// <summary>
/// Sets a room code for a room
/// </summary>
/// <param name="RoomId"></param>
/// <param name="RoomCode"></param>
public record SetRoomCodeCommand(long RoomId, string RoomCode) : ICommand;
