namespace TeaTime.Common.Features.Users.Queries;

using Abstractions;
using Models.Data;

public record GetUserQuery(long UserId) : IQuery<User?>;
