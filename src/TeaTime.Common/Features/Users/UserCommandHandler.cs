namespace TeaTime.Common.Features.Users
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using Commands;
    using MediatR;
    using Models.Data;

    public class UserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly TimeProvider _clock;

        public UserCommandHandler(IUserRepository userRepository, TimeProvider clock)
        {
            _userRepository = userRepository;
            _clock = clock;
        }

        public Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = request.Id,
                Username = request.Username,
                CreatedDate = _clock.GetUtcNow(),
                DisplayName = request.DisplayName
            };

            return _userRepository.CreateAsync(user);

            //todo: create event
        }
    }
}
