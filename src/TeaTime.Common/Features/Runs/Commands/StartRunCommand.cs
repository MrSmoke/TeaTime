namespace TeaTime.Common.Features.Runs.Commands;

using System;
using Abstractions;

/// <summary>
/// The command to start a teatime
/// </summary>
/// <param name="Id">The id of the run</param>
/// <param name="UserId">The user who is starting the tea time</param>
/// <param name="RoomId">The room the tea time will be run in</param>
/// <param name="RoomGroupId"></param>
/// <param name="StartTime"></param>
public record StartRunCommand(long Id, long UserId, long RoomId, long RoomGroupId, DateTimeOffset StartTime)
    : BaseCommand, IUserCommand;
