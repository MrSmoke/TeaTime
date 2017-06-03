namespace TeaTime.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions;
    using Models;

    public interface IRoomService : ILinkable<Room>
    {
        /// <summary>
        /// Create a new room
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Room> Create(string name);

        /// <summary>
        /// Get a room by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Room> Get(Guid id);

        /// <summary>
        /// Add an option to be displayed when a run for this room is created
        /// </summary>
        /// <returns></returns>
        Task<Option> AddOption(Room room, string group, string option);

        /// <summary>
        /// Remove an option from this room
        /// </summary>
        /// <returns></returns>
        Task RemoveOption(Room room, string group, Guid id);

        /// <summary>
        /// Gets all the room options by the given group name
        /// </summary>
        /// <param name="room"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        Task<IEnumerable<Option>> GetOptions(Room room, string group);
    }
}