namespace TeaTime.Common.Models.Data;

using System;

public record RunResultEntity
{
    public required long RunId { get; init; }

    /// <summary>
    /// The user id of the runner
    /// </summary>
    public required long RunnerUserId { get; init; }

    /// <summary>
    /// The time the run ended
    /// </summary>
    public required DateTimeOffset EndedTime { get; init; }
}
