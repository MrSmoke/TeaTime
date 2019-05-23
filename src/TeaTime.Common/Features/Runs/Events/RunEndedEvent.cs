namespace TeaTime.Common.Features.Runs.Events
{
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using Models.Domain;

    public class RunEndedEvent : Event
    {
        public long RunId { get; set; }
        public long RoomId { get; set; }
        public DateTimeOffset EndedTime { get; set; }

        public long RunnerUserId { get; set; }
        public IEnumerable<OrderModel> Orders { get; set; }
    }
}