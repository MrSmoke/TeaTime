namespace TeaTime.Common.Abstractions.Data
{
    using System.Threading.Tasks;
    using Models.Data;

    public interface IRunRepository
    {
        Task CreateAsync(Run run);
        Task<Run> GetAsync(long runId);

        Task CreateResultAsync(RunResult result);
    }
}
