namespace TeaTime.Common.Features.Users
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using AutoMapper;
    using Commands;
    using MediatR;
    using Models.Data;

    public class UserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ISystemClock _clock;

        public UserCommandHandler(IUserRepository userRepository, IMapper mapper, ISystemClock clock)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _clock = clock;
        }

        public Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<CreateUserCommand, User>(request);

            user.CreatedDate = _clock.UtcNow();

            return _userRepository.CreateAsync(user);

            //todo: create event
        }
    }
}
