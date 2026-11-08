namespace TeaTime.Common.Features.IllMake;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Commands;
using MediatR;
using Models.Data;

public class IllMakeCommandHandler(IIllMakeRepository repository, TimeProvider clock) : IRequestHandler<IllMakeCommand>
{
    public Task Handle(IllMakeCommand request, CancellationToken cancellationToken)
    {
        var model = new IllMakeEntity
        {
            Id = request.Id,
            RunId = request.RunId,
            UserId = request.CurrentUserId,
            CreatedDate = clock.GetUtcNow()
        };

        //todo: event
        return repository.CreateAsync(model, cancellationToken);
    }
}
