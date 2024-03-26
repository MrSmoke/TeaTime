namespace TeaTime.Common.Features.Runs.Queries;

using System.Collections.Generic;
using Abstractions;
using Models.Data;

public record GetRunsByRoomId(long RoomId, int Limit = 1) : IQuery<IEnumerable<Run>>;
