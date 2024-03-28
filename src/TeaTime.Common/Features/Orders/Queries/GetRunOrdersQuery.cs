namespace TeaTime.Common.Features.Orders.Queries;

using System.Collections.Generic;
using Abstractions;
using Models.Domain;

public record GetRunOrdersQuery(long RunId, long UserId) : IUserQuery<IEnumerable<OrderModel>>;
