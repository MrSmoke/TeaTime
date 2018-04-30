namespace TeaTime.Common.Features.Runs.Events
{
    using System;
    using Abstractions;

    public class RunStartedEvent : Event
    {
        public long RunId { get; set; }

        /// <summary>
        /// The id of the user who started the run
        /// </summary>
        public long UserId { get; set; }

        public long RoomId { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
    }
}
