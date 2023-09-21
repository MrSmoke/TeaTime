namespace TeaTime.Common.Features.Users.Queries
{
    using Abstractions;
    using Models.Data;

    public class GetUserQuery : IQuery<User?>
    {
        public long UserId { get; }

        public GetUserQuery(long userId)
        {
            UserId = userId;
        }
    }
}
