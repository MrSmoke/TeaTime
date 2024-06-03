namespace TeaTime.Common.Models.Data
{
    using System;

    public class RunResult
    {
        public long RunId { get; set; }

        /// <summary>
        /// The user id of the runner
        /// </summary>
        public long RunnerUserId { get; set; }

        /// <summary>
        /// The time the run ended
        /// </summary>
        public DateTimeOffset EndedTime { get; set; }
    }
}
