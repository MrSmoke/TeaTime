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
        private readonly IRoomRepository _roomRepository;
        private readonly IRunnerRandomizer _randomizer;
        private readonly IMapper _mapper;
        private readonly ISystemClock _clock;

        public RunCommandHandler(IRunRepository runRepository, IEventPublisher eventPublisher, IRoomRepository roomRepository, IRunnerRandomizer randomizer, IMapper mapper, ISystemClock clock)
        {
            _runRepository = runRepository;
            _eventPublisher = eventPublisher;
            _roomRepository = roomRepository;
            _randomizer = randomizer;
            _mapper = mapper;
            _clock = clock;
        }

        //Start run
        public async Task Handle(StartRunCommand request, CancellationToken cancellationToken)
        {
            await _roomRepository.CreateCurrentRunAsync(request.RoomId, request.Id).ConfigureAwait(false);

            var run = _mapper.Map<StartRunCommand, Run>(request);

            run.CreatedDate = _clock.UtcNow();

            await _runRepository.CreateAsync(run).ConfigureAwait(false);

            var evt = _mapper.Map<Run, RunStartedEvent>(run);

            await _eventPublisher.Publish(evt).ConfigureAwait(false);
        }

        //End run
        public async Task Handle(EndRunCommand request, CancellationToken cancellationToken)
        {
            var currentRunId = await _roomRepository.GetCurrentRunAsync(request.RoomId).ConfigureAwait(false);

            //random runner
            var runnerUserId = await _randomizer.GetRunnerUserId(request.Orders).ConfigureAwait(false);

            //store runner
            var runResult = new RunResult
            {
                RunId = currentRunId,
                RunnerUserId = runnerUserId,
                EndedTime = _clock.UtcNow()
            };

            await _runRepository.CreateResultAsync(runResult).ConfigureAwait(false);

            //mark run as ended
            await _roomRepository.DeleteCurrentRunAsync(request.RoomId).ConfigureAwait(false);

            //publish event
            var evt = new RunEndedEvent
            {
                RoomId = request.RoomId,
                Orders = request.Orders,

                RunnerUserId = runResult.RunnerUserId,
                RunId = runResult.RunId,
                EndedTime = runResult.EndedTime
            };

            await _eventPublisher.Publish(evt).ConfigureAwait(false);
        }
    }
}
