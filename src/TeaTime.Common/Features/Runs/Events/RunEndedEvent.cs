namespace TeaTime.Common.Features.Runs.Events;

using System;
using System.Collections.Generic;
using Abstractions;
using Models.Domain;

public record RunEndedEvent
(
    long RunId,
    long RoomId,
    DateTimeOffset EndedTime,
    long RunnerUserId,
    IEnumerable<OrderModel> Orders
) : BaseEvent;
