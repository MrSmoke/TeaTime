namespace TeaTime.Common.Models
{
    using System;

    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid RoomId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
