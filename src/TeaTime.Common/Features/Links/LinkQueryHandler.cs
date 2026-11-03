namespace TeaTime.Common.Features.Links;

using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using MediatR;
using Queries;

public class LinkQueryHandler(ILinkRepository linkRepository) : IRequestHandler<GetObjectIdByLinkValueQuery, long?>
{
    public Task<long?> Handle(GetObjectIdByLinkValueQuery request, CancellationToken cancellationToken)
    {
        return linkRepository.GetObjectIdAsync(request.Value, request.LinkType, cancellationToken);
    }
}
