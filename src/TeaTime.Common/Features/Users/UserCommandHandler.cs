namespace TeaTime.Common.Features.Users;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Commands;
using MediatR;
using Models.Data;

public class UserCommandHandler(IUserRepository userRepository, TimeProvider clock) : IRequestHandler<CreateUserCommand>
{
    public Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new UserEntity
        {
            Id = request.Id,
            Username = request.Username,
            CreatedDate = clock.GetUtcNow(),
            DisplayName = request.DisplayName
        };

        return userRepository.CreateAsync(user, cancellationToken);

        //todo: create event
    }
}
