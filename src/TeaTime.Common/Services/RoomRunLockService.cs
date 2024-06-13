namespace TeaTime.Common.Services;

using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Abstractions.Data;
using Microsoft.Extensions.Logging;

public class RoomRunLockService(ILockRepository lockRepository, ILogger<RoomRunLockService> logger)
    : IRoomRunLockService
{
    public async Task<bool> CreateLockAsync(long roomId, CancellationToken cancellationToken = default)
    {
        var key = GetLockKey(roomId);
        var result = await lockRepository.CreateLockAsync(key);

        logger.LogDebug("Create run lock for room {RoomId} with key {Key} result = {Result}", roomId, key, result);

        return result;
    }

    public async Task<bool> DeleteLockAsync(long roomId, CancellationToken cancellationToken = default)
    {
        var key = GetLockKey(roomId);
        var result = await lockRepository.DeleteLockAsync(key);

        logger.LogDebug("Delete run lock for room {RoomId} with key {Key} result = {Result}", roomId, key, result);

        return result;
    }

    private static string GetLockKey(long roomId)
    {
        return "room" + roomId + ":run";
    }
}
