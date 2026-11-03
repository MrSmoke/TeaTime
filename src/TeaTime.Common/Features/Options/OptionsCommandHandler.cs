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
    IRequestHandler<DeleteOptionCommand>
{
    public Task Handle(CreateOptionCommand request, CancellationToken cancellationToken)
    {
        var option = new Option
        {
            Id = request.Id,
            Name = request.Name,
            CreatedBy = request.UserId,
            CreatedDate = clock.GetUtcNow(),
            GroupId = request.GroupId
        };

        return optionsRepository.CreateAsync(option, cancellationToken);
    }

    public Task Handle(DeleteOptionCommand request, CancellationToken cancellationToken)
    {
        return optionsRepository.DeleteAsync(request.OptionId, cancellationToken);
    }
}
