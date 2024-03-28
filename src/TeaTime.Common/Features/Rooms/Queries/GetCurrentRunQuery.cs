namespace TeaTime.Common.Features.Rooms.Queries;

using Abstractions;
using Models.Data;

public record GetCurrentRunQuery(long RoomId, long UserId) : IUserQuery<Run?>;
