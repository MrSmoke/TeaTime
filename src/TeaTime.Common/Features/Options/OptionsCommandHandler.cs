namespace TeaTime.Common.Features.Options;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Commands;
using Exceptions;
using MediatR;
using Models.Data;

public class OptionsCommandHandler(TimeProvider clock, IOptionsRepository optionsRepository) :
    IRequestHandler<CreateOptionCommand>,
    IRequestHandler<DeleteOptionCommand>,
    IRequestHandler<SetOptionAvailabilityCommand>
{
    public async Task Handle(CreateOptionCommand request, CancellationToken cancellationToken)
    {
        var existing = await optionsRepository.GetOptionsByGroupIdAsync(request.GroupId, cancellationToken);

        // Names are matched ignoring case, so a second "earl grey" would make lookups ambiguous
        if (existing.Any(o => o.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new OptionException($"Option {request.Name} already exists in group {request.GroupId}",
                OptionException.OptionExceptionReason.DuplicateName);
        }

        var option = new OptionEntity
        {
            Id = request.Id,
            Name = request.Name,
            Available = true,
            GroupId = request.GroupId,
            CreatedBy = request.UserId,
            CreatedDate = clock.GetUtcNow(),
        };

        await optionsRepository.CreateAsync(option, cancellationToken);
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
