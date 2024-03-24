namespace TeaTime.Common.Features.Orders.Queries;

using Abstractions;
using Models.Data;

public record GetUserOrderQuery(long RunId, long UserId) : IUserQuery<Order?>;
