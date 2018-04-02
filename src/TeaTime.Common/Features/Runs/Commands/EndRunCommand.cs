namespace TeaTime.Common.Features.Runs.Commands
{
    using System.Collections.Generic;
    using Abstractions;
    using Models.Data;

    /// <summary>
    /// The command to end a run
    /// </summary>
    public class EndRunCommand : IUserCommand
    {
        public long RunId { get; }
        public long RoomId { get; }
        public long UserId { get; }

        public IEnumerable<Order> Orders { get; }

        public EndRunCommand(long runId, long roomId, long userId, IEnumerable<Order> orders)
        {
            RunId = runId;
            RoomId = roomId;
            UserId = userId;
            Orders = orders;
        }
    }
}