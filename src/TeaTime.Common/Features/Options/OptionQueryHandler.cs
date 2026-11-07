namespace TeaTime.Common.Features.Options;

using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using MediatR;
using Models.Data;
using Queries;

public class OptionQueryHandler(IOptionsRepository optionsRepository) : IRequestHandler<GetOptionQuery, OptionEntity?>
{
    public Task<OptionEntity?> Handle(GetOptionQuery request, CancellationToken cancellationToken)
    {
        return optionsRepository.GetAsync(request.Id, cancellationToken);
    }
}
