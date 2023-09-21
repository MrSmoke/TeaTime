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

        public RunCommandHandler(IRunRepository runRepository, IEventPublisher eventPublisher, IRunnerRandomizer randomizer, IMapper mapper, ISystemClock clock, IIllMakeRepository illMakeRepository)
        {
            _runRepository = runRepository;
            _eventPublisher = eventPublisher;
            _randomizer = randomizer;
            _mapper = mapper;
            _clock = clock;
            _illMakeRepository = illMakeRepository;
        }

        //Start run
        public async Task<Unit> Handle(StartRunCommand request, CancellationToken cancellationToken)
        {
            var run = _mapper.Map<StartRunCommand, Run>(request);

            run.CreatedDate = _clock.UtcNow();

            await _runRepository.CreateAsync(run);

            var evt = _mapper.Map<Run, RunStartedEvent>(run);

            await _eventPublisher.PublishAsync(evt);

            return Unit.Value;
        }

        //End run
        public async Task<Unit> Handle(EndRunCommand request, CancellationToken cancellationToken)
        {
            var run = await _runRepository.GetAsync(request.RunId);

            var runnerUserId = await GetRunner(request);

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
            {
                Orders = request.Orders,
                RoomId = request.RoomId,
                RunnerUserId = runResult.RunnerUserId,
                RunId = runResult.RunId,
                EndedTime = runResult.EndedTime,
                State = request.State
            };

            await _eventPublisher.PublishAsync(evt);

            return Unit.Value;
        }

        private async Task<long> GetRunner(EndRunCommand command)
        {
            //todo: dont tie illmake in with this handler directly...kinda gross
            var illMakeResults = await _illMakeRepository.GetAllByRunAsync(command.RunId);

            if (illMakeResults.Any())
                return illMakeResults.OrderByDescending(o => o.CreatedDate).First().UserId;

            //random runner
            return await _randomizer.GetRunnerUserId(command.Orders);
        }
    }
}
