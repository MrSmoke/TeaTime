namespace TeaTime.Common.Features.Rooms.Events
{
    using Abstractions;

    public class RoomCreatedEvent : Event
    {
        public long Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// The id of the user who created the room
        /// </summary>
        public long CreatedBy { get; set; }

        /// <summary>
        /// Set to true to create the default item groups for this room
        /// </summary>
        public bool CreateDefaultItemGroup { get; set; }
    }
}
