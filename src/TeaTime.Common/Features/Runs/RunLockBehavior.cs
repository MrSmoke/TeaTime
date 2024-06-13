namespace TeaTime.Common.Features.Runs;

using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Commands;
using Exceptions;
using MediatR;

public class RunLockBehavior<TCommand, TResponse>(IRoomRunLockService lockService)
    : IPipelineBehavior<TCommand, TResponse>
    where TCommand : notnull
{
    public async Task<TResponse> Handle(TCommand request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        await ProcessAsync(request, cancellationToken);

        return await next();
    }

    private Task ProcessAsync(TCommand request, CancellationToken cancellationToken)
    {
        return request switch
        {
            StartRunCommand startRunCommand => ProcessAsync(startRunCommand, cancellationToken),
            EndRunCommand endRunCommand => ProcessAsync(endRunCommand, cancellationToken),
            _ => Task.CompletedTask
        };
    }

    private async Task ProcessAsync(StartRunCommand request, CancellationToken cancellationToken)
    {
        //try to create the run lock
        var created = await lockService.CreateLockAsync(request.RoomId, cancellationToken);
        if (!created)
            throw new RunStartException("There is already an active run in this room",
                RunStartException.RunStartExceptionReason.ExistingActiveRun);

        //run lock created, so everything is ok
    }

    private async Task ProcessAsync(EndRunCommand request, CancellationToken cancellationToken)
    {
        //delete lock
        if (await lockService.DeleteLockAsync(request.Run.RoomId, cancellationToken))
            return;

        throw new RunEndException("There is no active run in this room",
            RunEndException.RunEndExceptionReason.NoActiveRun);
    }
}
