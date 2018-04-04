namespace TeaTime.Common.Features.Runs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using AutoMapper;
    using Commands;
    using Events;
    using MediatR;
    using Models.Data;

    public class RunCommandHandler :
        IRequestHandler<StartRunCommand>,
        IRequestHandler<EndRunCommand>
    {
        private readonly IRunRepository _runRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRunnerRandomizer _randomizer;
        private readonly IMapper _mapper;
        private readonly ISystemClock _clock;

        public RunCommandHandler(IRunRepository runRepository, IEventPublisher eventPublisher, IRunnerRandomizer randomizer, IMapper mapper, ISystemClock clock)
        {
            _runRepository = runRepository;
            _eventPublisher = eventPublisher;
            _randomizer = randomizer;
            _mapper = mapper;
            _clock = clock;
        }

        //Start run
        public async Task Handle(StartRunCommand request, CancellationToken cancellationToken)
        {
            var run = _mapper.Map<StartRunCommand, Run>(request);

            run.CreatedDate = _clock.UtcNow();

            await _runRepository.CreateAsync(run).ConfigureAwait(false);

            var evt = _mapper.Map<Run, RunStartedEvent>(run);

            await _eventPublisher.Publish(evt).ConfigureAwait(false);
        }

        //End run
        public async Task Handle(EndRunCommand request, CancellationToken cancellationToken)
        {
            var run = await _runRepository.GetAsync(request.RunId).ConfigureAwait(false);

            //random runner
            var runnerUserId = await _randomizer.GetRunnerUserId(request.Orders).ConfigureAwait(false);

            //update run
            run.Ended = true;
            await _runRepository.UpdateAsync(run).ConfigureAwait(false);

            //store result
            var runResult = new RunResult
            {
                RunId = request.RunId,
                RunnerUserId = runnerUserId,
                EndedTime = _clock.UtcNow()
            };

            await _runRepository.CreateResultAsync(runResult).ConfigureAwait(false);

            //publish event
            var evt = new RunEndedEvent
            {
                Orders = request.Orders,
                RoomId = request.RoomId,
                RunnerUserId = runResult.RunnerUserId,
                RunId = runResult.RunId,
                EndedTime = runResult.EndedTime
            };

            await _eventPublisher.Publish(evt).ConfigureAwait(false);
        }
    }
}
