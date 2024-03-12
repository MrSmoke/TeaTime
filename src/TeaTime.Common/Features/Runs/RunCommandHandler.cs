namespace TeaTime.Common.Features.Runs
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Data;
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
        private readonly ISystemClock _clock;
        private readonly IIllMakeRepository _illMakeRepository;

        public RunCommandHandler(IRunRepository runRepository,
            IEventPublisher eventPublisher,
            IRunnerRandomizer randomizer,
            ISystemClock clock,
            IIllMakeRepository illMakeRepository)
        {
            _runRepository = runRepository;
            _eventPublisher = eventPublisher;
            _randomizer = randomizer;
            _clock = clock;
            _illMakeRepository = illMakeRepository;
        }

        //Start run
        public async Task Handle(StartRunCommand request, CancellationToken cancellationToken)
        {
            var run = new Run
            {
                Id = request.Id,
                CreatedDate = _clock.UtcNow(),
                GroupId = request.RoomGroupId,
                RoomId = request.RoomId,
                StartTime = request.StartTime,
                UserId = request.UserId
            };

            await _runRepository.CreateAsync(run);

            var evt = new RunStartedEvent
            (
                RunId: run.Id,
                UserId: run.UserId,
                RoomId: run.RoomId,
                StartTime: run.StartTime,
                EndTime: run.EndTime
            );

            await _eventPublisher.PublishAsync(evt);
        }

        //End run
        public async Task Handle(EndRunCommand request, CancellationToken cancellationToken)
        {
            var runnerUserId = await GetRunner(request);

            //update run
            await _runRepository.UpdateAsync(request.Run with
            {
                Ended = true
            });

            //store result
            var runResult = new RunResult
            {
                RunId = request.Run.Id,
                RunnerUserId = runnerUserId,
                EndedTime = _clock.UtcNow()
            };

            await _runRepository.CreateResultAsync(runResult);

            //publish event
            var evt = new RunEndedEvent
            (
                Orders: request.Orders,
                RoomId: request.Run.RoomId,
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
            var illMakeResults = await _illMakeRepository.GetAllByRunAsync(command.Run.Id);

            var illMakeUser = illMakeResults.MaxBy(o => o.CreatedDate);

            if (illMakeUser is not null)
                return illMakeUser.Id;

            //random runner
            return await _randomizer.GetRunnerUserId(command.Orders);
        }
    }
}
