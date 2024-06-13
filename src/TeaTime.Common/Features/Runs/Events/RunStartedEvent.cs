namespace TeaTime.Common.Features.Runs.Events;

using System;
using Abstractions;

/// <param name="RunId"></param>
/// <param name="UserId">The id of the user who started the run</param>
/// <param name="RoomId"></param>
/// <param name="StartTime"></param>
/// <param name="EndTime"></param>
public record RunStartedEvent(
    long RunId,
    long UserId,
    long RoomId,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
) : BaseEvent;
