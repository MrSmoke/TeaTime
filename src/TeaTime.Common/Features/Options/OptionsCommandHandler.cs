namespace TeaTime.Common.Features.Options
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using AutoMapper;
    using Commands;
    using MediatR;
    using Models.Data;

    public class OptionsCommandHandler :
        IRequestHandler<CreateOptionCommand>,
        IRequestHandler<DeleteOptionCommand>
    {
        private readonly ISystemClock _clock;
        private readonly IOptionsRepository _optionsRepository;
        private readonly IMapper _mapper;

        public OptionsCommandHandler(ISystemClock clock, IOptionsRepository optionsRepository, IMapper mapper)
        {
            _clock = clock;
            _optionsRepository = optionsRepository;
            _mapper = mapper;
        }

        public Task Handle(CreateOptionCommand request, CancellationToken cancellationToken)
        {
            var option = _mapper.Map<CreateOptionCommand, Option>(request);

            option.CreatedDate = _clock.UtcNow();

            return _optionsRepository.CreateAsync(option);
        }

        public Task Handle(DeleteOptionCommand request, CancellationToken cancellationToken)
        {
            return _optionsRepository.DeleteAsync(request.OptionId);
        }
    }
}
