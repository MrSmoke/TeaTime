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

        public Task<bool> CreateLockAsync(long roomId)
        {
            return _lockRepository.CreateLockAsync(GetLockKey(roomId));
        }

        public Task<bool> DeleteLockAsync(long roomId)
        {
            return _lockRepository.DeleteLockAsync(GetLockKey(roomId));
        }

        private static string GetLockKey(long roomId)
        {
            return "room" + roomId + ":run";
        }
    }
}
