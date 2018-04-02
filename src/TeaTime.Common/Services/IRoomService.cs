//namespace TeaTime.Common.Services
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Threading.Tasks;
//    using Abstractions;
//    using Models;
//    using Models.Data;

//    public interface IRoomService : ILinkable<Room>
//    {
//        /// <summary>
//        /// Create a new room
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        Task<Room> Create(string name);

//        /// <summary>
//        /// Get a room by its Id
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        Task<Room> Get(Guid id);

//        Task<RoomGroup> AddGroup(Room room, string name);

//        Task<RoomGroup> GetGroupByName(Room room, string name);

//        /// <summary>
//        /// Add an option to be displayed when a run for this room is created
//        /// </summary>
//        /// <returns></returns>
//        Task<Option> AddOption(RoomGroup group, string option);

//        /// <summary>
//        /// Remove an option from this room
//        /// </summary>
//        /// <returns></returns>
//        Task RemoveOption(Guid optionId);

//        /// <summary>
//        /// Gets all the room options by the given group name
//        /// </summary>
//        /// <param name="room"></param>
//        /// <param name="group"></param>
//        /// <returns></returns>
//        Task<IEnumerable<Option>> GetOptions(RoomGroup group);
//    }
//}