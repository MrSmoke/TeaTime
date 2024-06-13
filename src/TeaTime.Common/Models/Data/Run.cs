namespace TeaTime.Common.Models.Data;

using System;

public record Run : BaseDataObject
{
    /// <summary>
    /// The room in which the run is associated to
    /// </summary>
    public required long RoomId { get; init; }

    /// <summary>
    /// The user who started the run
    /// </summary>
    public required long UserId { get; init; }

    public required long GroupId { get; init; }

    /// <summary>
    /// The time the run was started
    /// </summary>
    public required DateTimeOffset StartTime { get; init; }

    /// <summary>
    /// The end time of the run (optional)
    /// </summary>
    public DateTimeOffset? EndTime { get; init; }

    /// <summary>
    /// True if the run has ended
    /// </summary>
    public bool Ended { get; init; }
}
