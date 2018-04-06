namespace TeaTime.Common.Abstractions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Domain;

    public interface IRunnerRandomizer
    {
        Task<long> GetRunnerUserId(IEnumerable<OrderModel> orders);
    }
}