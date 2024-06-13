namespace TeaTime.Common.Features.IllMake
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using Commands;
    using MediatR;
    using Models.Data;

    public class IllMakeCommandHandler : IRequestHandler<IllMakeCommand>
    {
        private readonly IIllMakeRepository _repository;
        private readonly TimeProvider _clock;

        public IllMakeCommandHandler(IIllMakeRepository repository, TimeProvider clock)
        {
            _repository = repository;
            _clock = clock;
        }

        public Task Handle(IllMakeCommand request, CancellationToken cancellationToken)
        {
            var model = new IllMake
            {
                Id = request.Id,
                RunId = request.RunId,
                UserId = request.UserId,
                CreatedDate = _clock.GetUtcNow()
            };

            //todo: event
            return _repository.CreateAsync(model);
        }
    }
}
