namespace TeaTime.Common.Features.Orders.Queries
{
    using System.Collections.Generic;
    using Abstractions;
    using Models.Domain;

    public class GetRunOrdersQuery : IUserQuery<IEnumerable<OrderModel>>
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
