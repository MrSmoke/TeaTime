namespace TeaTime.Common.Models
{
    using System;

    public class Option
    {
        public Guid Id { get; set; }
        public string Text { get; set; }

        public Guid RoomGroupId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
