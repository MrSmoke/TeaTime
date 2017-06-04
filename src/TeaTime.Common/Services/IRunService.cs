namespace TeaTime.Common.Services
{
    using System.Threading.Tasks;
    using Models;
    using Models.Results;

    public interface IRunService
    {
        /// <summary>
        /// Start a run
        /// </summary>
        /// <param name="room"></param>
        /// <param name="user"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        Task<Run> Start(Room room, User user, Group group);

        /// <summary>
        /// Join a run
        /// </summary>
        /// <param name="room"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> Join(Room room, User user);

        /// <summary>
        /// Join a run with a given option
        /// </summary>
        /// <param name="room"></param>
        /// <param name="user"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        Task<bool> Join(Room room, User user, Option option);

        /// <summary>
        /// Leaave the run
        /// </summary>
        /// <param name="room"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> Leave(Room room, User user);

        /// <summary>
        /// Volunteer to complete the run
        /// </summary>
        /// <param name="room"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> Volunteer(Room room, User user);

        /// <summary>
        /// The the run and choose a runner
        /// </summary>
        /// <param name="room"></param>
        /// <param name="user">The user ending the run</param>
        /// <returns></returns>
        Task<RunEndResult> End(Room room, User user);

        /// <summary>
        /// Gets a currently running run in a room
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        Task<Run> GetCurrent(Room room);
    }
}