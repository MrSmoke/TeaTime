namespace TeaTime.Common.Features.Users;

using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using MediatR;
using Models.Data;
using Queries;

public class UserQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserQuery, User?>
{
    public Task<User?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return userRepository.GetAsync(request.UserId, cancellationToken);
    }
}
