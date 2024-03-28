namespace TeaTime.Common.Features.RoomItemGroups.Queries;

using Abstractions;
using Models;

public record GetRoomItemGroupQuery(long RoomId, long UserId, long GroupId) : IUserQuery<RoomItemGroupModel?>;
