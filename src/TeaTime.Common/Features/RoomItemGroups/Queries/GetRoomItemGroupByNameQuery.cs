namespace TeaTime.Common.Features.RoomItemGroups.Queries;

using Abstractions;
using Models;

public record GetRoomItemGroupByNameQuery(long RoomId, long UserId, string Name) : IUserQuery<RoomItemGroupModel?>;
