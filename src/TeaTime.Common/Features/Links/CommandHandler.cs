namespace TeaTime.Common.Features.Links;

using System.Threading;
using System.Threading.Tasks;
using Abstractions.Data;
using Commands;
using MediatR;

public class LinkCommandHandler(ILinkRepository linkRepository) : IRequestHandler<CreateLinkCommand>
{
    public Task Handle(CreateLinkCommand request, CancellationToken cancellationToken)
    {
        return linkRepository.AddAsync(request.ObjectId, request.LinkType, request.Value, cancellationToken);
    }
}
