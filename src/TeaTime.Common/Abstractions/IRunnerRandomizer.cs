namespace TeaTime.Common.Abstractions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Data;

    public interface IRunnerRandomizer
    {
        Task<long> GetRunnerUserId(IEnumerable<Order> orders);
    }
}