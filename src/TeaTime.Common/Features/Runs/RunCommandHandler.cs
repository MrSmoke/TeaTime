namespace TeaTime.Common.Features.Runs
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
    using AutoMapper;
    using Commands;
    using Events;
    using MediatR;
    using Microsoft.Extensions.Logging;
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
        private readonly IIllMakeRepository _illMakeRepository;
        private readonly ILogger<RunCommandHandler> _logger;

        public RunCommandHandler(IRunRepository runRepository,
            IEventPublisher eventPublisher,
            IRunnerRandomizer randomizer,
            IMapper mapper,
            ISystemClock clock,
            IIllMakeRepository illMakeRepository,
            ILogger<RunCommandHandler> logger)
        {
            _runRepository = runRepository;
            _eventPublisher = eventPublisher;
            _randomizer = randomizer;
            _mapper = mapper;
            _clock = clock;
            _illMakeRepository = illMakeRepository;
            _logger = logger;
        }

        //Start run
        public async Task Handle(StartRunCommand request, CancellationToken cancellationToken)
        {
            var run = _mapper.Map<StartRunCommand, Run>(request);

            run.CreatedDate = _clock.UtcNow();

            await _runRepository.CreateAsync(run);

            var evt = _mapper.Map<Run, RunStartedEvent>(run);

            await _eventPublisher.PublishAsync(evt);
        }

        //End run
        public async Task Handle(EndRunCommand request, CancellationToken cancellationToken)
        {
            var runTask = _runRepository.GetAsync(request.RunId);
            var runnerUserId = await GetRunner(request);

            var run = await runTask;

            if (run is null)
            {
                _logger.LogWarning("Failed to get run {RunId}", request.RunId);
                return;
            }

            //update run
            run.Ended = true;
            await _runRepository.UpdateAsync(run);

            //store result
            var runResult = new RunResult
            {
                RunId = request.RunId,
                RunnerUserId = runnerUserId,
                EndedTime = _clock.UtcNow()
            };

            await _runRepository.CreateResultAsync(runResult);

            //publish event
            var evt = new RunEndedEvent
            (
                Orders: request.Orders,
                RoomId: request.RoomId,
                RunnerUserId: runResult.RunnerUserId,
                RunId: runResult.RunId,
                EndedTime: runResult.EndedTime
            )
            {
                State = request.State
            };

            await _eventPublisher.PublishAsync(evt);
        }

        private async Task<long> GetRunner(EndRunCommand command)
        {
            //todo: dont tie illmake in with this handler directly...kinda gross
            var illMakeResults = await _illMakeRepository.GetAllByRunAsync(command.RunId);

            var illMakeUser = illMakeResults.MaxBy(o => o.CreatedDate);

            if (illMakeUser is not null)
                return illMakeUser.Id;

            //random runner
            return await _randomizer.GetRunnerUserId(command.Orders);
        }
    }
}
