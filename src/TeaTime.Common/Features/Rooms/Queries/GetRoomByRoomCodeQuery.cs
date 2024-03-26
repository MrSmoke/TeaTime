namespace TeaTime.Common.Features.Rooms.Queries;

using Abstractions;
using Models.Data;

public record GetRoomByRoomCodeQuery(string RoomCode) : IQuery<Room?>;
