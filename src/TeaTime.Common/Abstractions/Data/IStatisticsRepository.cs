namespace TeaTime.Common.Abstractions.Data
{
    using System.Threading.Tasks;

    public interface IStatisticsRepository
    {
        /// <summary>
        /// Gets the count of all orders made from ended runs
        /// </summary>
        /// <returns></returns>
        Task<long> CountGlobalOrdersMadeAsync();

        /// <summary>
        /// Gets the count of all runs which have ended
        /// </summary>
        /// <returns></returns>
        Task<long> CountGlobalEndedRunsAsync();
    }
}