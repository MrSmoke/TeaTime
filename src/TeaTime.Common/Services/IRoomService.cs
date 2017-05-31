namespace TeaTime.Common.Services
{
    using System.Threading.Tasks;
    using Abstractions;
    using Models;

    public interface IRoomService : ILinkable<Room>
    {
        /// <summary>
        /// Create a new room
        /// </summary>
        /// <returns></returns>
        Task<Room> Create();

        /// <summary>
        /// Add an option to be displayed when a run for this room is created
        /// </summary>
        /// <returns></returns>
        Task AddOption(Room room, string option);

        /// <summary>
        /// Remove an option from this room
        /// </summary>
        /// <returns></returns>
        Task RemoveOption(Room room, string option);
    }
}