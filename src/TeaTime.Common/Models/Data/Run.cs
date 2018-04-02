namespace TeaTime.Common.Models.Data
{
    using System;

    public class Run : BaseDataObject
    {
        /// <summary>
        /// The room in which the run is associated to
        /// </summary>
        public long RoomId { get; set; }

        /// <summary>
        /// The user who started the run
        /// </summary>
        public long UserId { get; set; }

        public long GroupId { get; set; }

        /// <summary>
        /// The time the run was started
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// The end time of the run (optional)
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }
    }
}