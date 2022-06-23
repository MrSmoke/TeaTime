namespace TeaTime.Common.Features.Orders.Queries
{
    using Abstractions;
    using Models.Data;

    public class GetUserOrderQuery : IUserQuery<Order?>
    {
        public long UserId { get; }
        public long RunId { get; }

        public GetUserOrderQuery(long runId, long userId)
        {
            RunId = runId;
            UserId = userId;
        }
    }
}
