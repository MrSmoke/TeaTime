namespace TeaTime.Common.Features.Runs;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Abstractions.Data;
using Commands;
using Events;
using MediatR;
using Models.Data;

public class RunCommandHandler(
    IRunRepository runRepository,
    IEventPublisher eventPublisher,
    IRunnerRandomizer randomizer,
    TimeProvider clock,
    IIllMakeRepository illMakeRepository) :
    IRequestHandler<StartRunCommand>,
    IRequestHandler<EndRunCommand>
{
    //Start run
    public async Task Handle(StartRunCommand request, CancellationToken cancellationToken)
    {
        var run = new Run
        {
            Id = request.Id,
            CreatedDate = clock.GetUtcNow(),
            GroupId = request.RoomGroupId,
            RoomId = request.RoomId,
            StartTime = request.StartTime,
            UserId = request.UserId
        };

        await runRepository.CreateAsync(run, cancellationToken);

        var evt = new RunStartedEvent
        (
            RunId: run.Id,
            UserId: run.UserId,
            RoomId: run.RoomId,
            StartTime: run.StartTime,
            EndTime: run.EndTime
        );

        await eventPublisher.PublishAsync(evt, cancellationToken);
    }

    //End run
    public async Task Handle(EndRunCommand request, CancellationToken cancellationToken)
    {
        var runnerUserId = await GetRunnerAsync(request, cancellationToken);

        //update run
        await runRepository.UpdateAsync(request.Run with
        {
            Ended = true
        }, cancellationToken);

        //store result
        var runResult = new RunResult
        {
            RunId = request.Run.Id,
            RunnerUserId = runnerUserId,
            EndedTime = clock.GetUtcNow()
        };

        await runRepository.CreateResultAsync(runResult, cancellationToken);

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

        await eventPublisher.PublishAsync(evt, cancellationToken);
    }

    private async Task<long> GetRunnerAsync(EndRunCommand command, CancellationToken cancellationToken = default)
    {
        //todo: dont tie illmake in with this handler directly...kinda gross
        var illMakeResults = await illMakeRepository.GetAllByRunAsync(command.Run.Id, cancellationToken);

        var illMake = illMakeResults.MaxBy(o => o.CreatedDate);

        if (illMake is not null)
            return illMake.UserId;

        //random runner
        return await randomizer.GetRunnerUserIdAsync(command.Orders, cancellationToken);
    }
}
