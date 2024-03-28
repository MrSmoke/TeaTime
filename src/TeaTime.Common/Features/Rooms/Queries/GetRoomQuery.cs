namespace TeaTime.Common.Features.Rooms.Queries;

using Abstractions;
using Models.Data;

public record GetRoomQuery(long RoomId) : IQuery<Room?>;
