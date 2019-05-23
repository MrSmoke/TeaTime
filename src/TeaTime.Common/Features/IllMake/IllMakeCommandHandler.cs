namespace TeaTime.Common.Features.IllMake
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using Commands;
    using MediatR;
    using Models.Data;

    public class IllMakeCommandHandler : IRequestHandler<IllMakeCommand>
    {
        private readonly IIllMakeRepository _repository;
        private readonly ISystemClock _clock;

        public IllMakeCommandHandler(IIllMakeRepository repository, ISystemClock clock)
        {
            _repository = repository;
            _clock = clock;
        }

        public async Task<Unit> Handle(IllMakeCommand request, CancellationToken cancellationToken)
        {
            var model = new IllMake
            {
                Id = request.Id,
                RunId = request.RunId,
                UserId = request.UserId,
                CreatedDate = _clock.UtcNow()
            };

            //todo: event
            await _repository.CreateAsync(model);

            return Unit.Value;
        }
    }
}
