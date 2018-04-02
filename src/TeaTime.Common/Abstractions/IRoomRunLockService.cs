﻿namespace TeaTime.Common.Abstractions
{
    using System.Threading.Tasks;

    public interface IRoomRunLockService
    {
        Task<bool> CreateLockAsync(long runId, long roomId);
        Task<bool> DeleteLockAsync(long runId, long roomId);
    }
}
