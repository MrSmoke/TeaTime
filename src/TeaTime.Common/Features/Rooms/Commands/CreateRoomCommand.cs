namespace TeaTime.Common.Features.Rooms.Commands
{
    using Abstractions;

    public class CreateRoomCommand : IUserCommand
    {
        public long Id { get; }
        public string Name { get; }

        /// <summary>
        /// The user id of the user who created the room
        /// </summary>
        public long UserId { get; }

        /// <summary>
        /// Set to true to create the default item groups for this room
        /// </summary>
        public bool CreateDefaultItemGroup { get; }

        public CreateRoomCommand(long id, string name, long userId, bool createDefaultItemGroup = true)
        {
            Id = id;
            Name = name;
            UserId = userId;
            CreateDefaultItemGroup = createDefaultItemGroup;
        }
    }
}
