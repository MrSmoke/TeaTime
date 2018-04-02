namespace TeaTime.Common.Features.Rooms.Commands
{
    using Abstractions;

    public class CreateRoomCommand : IUserCommand
    {
        public long Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// The user id of the user who created the room
        /// </summary>
        public long UserId { get; set; }

        public CreateRoomCommand(long id, string name, long userId)
        {
            Id = id;
            Name = name;
            UserId = userId;
        }
    }
}
