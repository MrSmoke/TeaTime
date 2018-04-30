namespace TeaTime.Common.Abstractions.Data
{
    using System.Threading.Tasks;

    public interface ILockRepository
    {
        Task<bool> CreateLockAsync(string key);
        Task<bool> DeleteLockAsync(string key);
    }
}
