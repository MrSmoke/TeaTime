namespace TeaTime.Common.Abstractions.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Data;

    public interface IRunRepository
    {
        Task CreateAsync(Run run);
        Task<Run?> GetAsync(long runId);
        Task<IEnumerable<Run>> GetManyByRoomId(long roomId, int limit = 10, bool? ended = null);
        Task UpdateAsync(Run run);

        Task CreateResultAsync(RunResult result);
    }
}
