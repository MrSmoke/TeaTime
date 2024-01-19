namespace TeaTime.Common.Features.Options
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using Commands;
    using MediatR;
    using Models.Data;

    public class OptionsCommandHandler :
        IRequestHandler<CreateOptionCommand>,
        IRequestHandler<DeleteOptionCommand>
    {
        private readonly ISystemClock _clock;
        private readonly IOptionsRepository _optionsRepository;

        public OptionsCommandHandler(ISystemClock clock, IOptionsRepository optionsRepository)
        {
            _clock = clock;
            _optionsRepository = optionsRepository;
        }

        public Task Handle(CreateOptionCommand request, CancellationToken cancellationToken)
        {
            var option = new Option
            {
                Id = request.Id,
                Name = request.Name,
                CreatedBy = request.UserId,
                CreatedDate = _clock.UtcNow(),
                GroupId = request.GroupId
            };

            return _optionsRepository.CreateAsync(option);
        }

        public Task Handle(DeleteOptionCommand request, CancellationToken cancellationToken)
        {
            return _optionsRepository.DeleteAsync(request.OptionId);
        }
    }
}
