namespace TeaTime.Common.Features.Runs.Queries;

using Abstractions;
using Models.Data;

public record GetCurrentRunQuery(long RoomId, long UserId) : IUserQuery<Run?>;
