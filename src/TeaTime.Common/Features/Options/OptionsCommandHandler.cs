namespace TeaTime.Common.Features.Options;

using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Commands;
using MediatR;
using Models.Data;

public class OptionsCommandHandler(TimeProvider clock, IOptionsRepository optionsRepository) :
    IRequestHandler<CreateOptionCommand>,
    IRequestHandler<DeleteOptionCommand>,
    IRequestHandler<SetOptionAvailabilityCommand>
{
    public Task Handle(CreateOptionCommand request, CancellationToken cancellationToken)
    {
        var option = new Option
        {
            Id = request.Id,
            Name = request.Name,
            Available = true,
            GroupId = request.GroupId,
            CreatedBy = request.UserId,
            CreatedDate = clock.GetUtcNow(),
        };

        return optionsRepository.CreateAsync(option, cancellationToken);
    }

    public Task Handle(DeleteOptionCommand request, CancellationToken cancellationToken)
    {
        return optionsRepository.DeleteAsync(request.OptionId, cancellationToken);
    }

    public Task Handle(SetOptionAvailabilityCommand request, CancellationToken cancellationToken)
    {
        return optionsRepository.SetAvailabilityAsync(request.OptionId, request.Available, cancellationToken);
    }
}
