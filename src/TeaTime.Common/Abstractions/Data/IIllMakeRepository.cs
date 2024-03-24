namespace TeaTime.Common.Abstractions.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Data;

    public interface IIllMakeRepository
    {
        Task CreateAsync(IllMake obj);
        Task<IEnumerable<IllMake>> GetAllByRunAsync(long runId);
    }
}
