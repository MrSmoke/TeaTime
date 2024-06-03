namespace TeaTime.Common.Services
{
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using Microsoft.Extensions.Logging;

    public class RoomRunLockService : IRoomRunLockService
    {
        private readonly ILockRepository _lockRepository;
        private readonly ILogger<RoomRunLockService> _logger;

        public RoomRunLockService(ILockRepository lockRepository, ILogger<RoomRunLockService> logger)
        {
            _lockRepository = lockRepository;
            _logger = logger;
        }

        public async Task<bool> CreateLockAsync(long roomId)
        {
            var key = GetLockKey(roomId);
            var result = await _lockRepository.CreateLockAsync(key);

            _logger.LogDebug("Create run lock for room {RoomId} with key {Key} result = {Result}", roomId, key, result);

            return result;
        }

        public async Task<bool> DeleteLockAsync(long roomId)
        {
            var key = GetLockKey(roomId);
            var result = await _lockRepository.DeleteLockAsync(key);

            _logger.LogDebug("Delete run lock for room {RoomId} with key {Key} result = {Result}", roomId, key, result);

            return result;
        }

        private static string GetLockKey(long roomId)
        {
            return "room" + roomId + ":run";
        }
    }
}
