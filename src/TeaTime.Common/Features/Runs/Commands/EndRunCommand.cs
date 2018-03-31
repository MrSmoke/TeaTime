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
        public long RoomId { get; set; }

        public long UserId { get; set; }

        public IEnumerable<Order> Orders { get; set; }
    }
}