﻿namespace TeaTime.Common.Features.Runs.Commands
{
    using System;
    using Abstractions;

    /// <summary>
    /// The command to start a teatime
    /// </summary>
    public class StartRunCommand : BaseCommand, IUserCommand
    {
        /// <summary>
        /// The id of the run
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// The user who is starting the tea time
        /// </summary>
        public long UserId { get; }

        /// <summary>
        /// The room the tea time will be run in
        /// </summary>
        public long RoomId { get; }

        public long RoomGroupId { get; }

        public DateTimeOffset StartTime { get; }

        public StartRunCommand(long id, long userId, long roomId, long roomGroupId, DateTimeOffset startTime)
        {
            Id = id;
            UserId = userId;
            RoomId = roomId;
            RoomGroupId = roomGroupId;
            StartTime = startTime;
        }
    }
}
