namespace TeaTime.Common.Features.Users
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using Commands;
    using MediatR;
    using Models.Data;

    public class UserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ISystemClock _clock;

        public UserCommandHandler(IUserRepository userRepository, ISystemClock clock)
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
                CreatedDate = _clock.UtcNow(),
                DisplayName = request.DisplayName
            };

            return _userRepository.CreateAsync(user);

            //todo: create event
        }
    }
}
