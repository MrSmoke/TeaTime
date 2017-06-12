namespace TeaTime.Common.Models
{
    using System;

    public class Option
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid RoomGroupId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
