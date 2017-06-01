namespace TeaTime.Common.Models
{
    using System;

    public class Run
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The room in which the run is associated to
        /// </summary>
        public Guid RoomId { get; set; }

        /// <summary>
        /// The user who started the run
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The time the run was started
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The end time of the run (optional)
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}