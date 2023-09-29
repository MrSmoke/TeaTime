namespace TeaTime.Common.Features.Links
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using Commands;
    using MediatR;

    public class LinkCommandHandler : IRequestHandler<CreateLinkCommand>
    {
        private readonly ILinkRepository _linkRepository;

        public LinkCommandHandler(ILinkRepository linkRepository)
        {
            _linkRepository = linkRepository;
        }

        public Task Handle(CreateLinkCommand request, CancellationToken cancellationToken)
        {
            return _linkRepository.Add(request.ObjectId, request.LinkType, request.Value);
        }
    }
}
