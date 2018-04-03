namespace TeaTime.Common.Services
{
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;

    public class RoomRunLockService : IRoomRunLockService
    {
        private readonly ILockRepository _lockRepository;

        public RoomRunLockService(ILockRepository lockRepository)
        {
            _lockRepository = lockRepository;
        }

        public Task<bool> CreateLockAsync(long runId, long roomId)
        {
            return _lockRepository.CreateLockAsync(GetLockKey(runId, roomId));
        }

        public Task<bool> DeleteLockAsync(long runId, long roomId)
        {
            return _lockRepository.DeleteLockAsync(GetLockKey(runId, roomId));
        }

        private static string GetLockKey(long runId, long roomId)
        {
            return "run:" + runId + ":room" + roomId;
        }
    }
}
