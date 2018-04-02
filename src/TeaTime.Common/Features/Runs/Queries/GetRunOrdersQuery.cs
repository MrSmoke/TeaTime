namespace TeaTime.Common.Features.Runs.Queries
{
    using System.Collections;
    using System.Collections.Generic;
    using Abstractions;
    using Models.Data;

    public class GetRunOrdersQuery : IUserQuery<IEnumerable<Order>>
    {
        public long RunId { get; }
        public long UserId { get; }

        public GetRunOrdersQuery(long runId, long userId)
        {
            RunId = runId;
            UserId = userId;
        }
    }
}
