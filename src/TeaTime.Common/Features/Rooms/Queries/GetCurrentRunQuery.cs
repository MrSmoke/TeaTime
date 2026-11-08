namespace TeaTime.Common.Features.Rooms.Queries;

using Abstractions;
using Models.Domain;

public record GetCurrentRunQuery(long RoomId, long UserId) : IUserQuery<CurrentRun?>;
