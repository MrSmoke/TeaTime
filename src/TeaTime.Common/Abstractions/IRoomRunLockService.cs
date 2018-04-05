namespace TeaTime.Common.Abstractions
{
    using System.Threading.Tasks;

    public interface IRoomRunLockService
    {
        Task<bool> CreateLockAsync(long roomId);
        Task<bool> DeleteLockAsync(long roomId);
    }
}
