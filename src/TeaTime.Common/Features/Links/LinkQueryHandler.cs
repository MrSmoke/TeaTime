namespace TeaTime.Common.Features.Links
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using MediatR;
    using Queries;

    public class LinkQueryHandler : IRequestHandler<GetObjectIdByLinkValueQuery, long?>
    {
        private readonly ILinkRepository _linkRepository;

        public LinkQueryHandler(ILinkRepository linkRepository)
        {
            _linkRepository = linkRepository;
        }

        public Task<long?> Handle(GetObjectIdByLinkValueQuery request, CancellationToken cancellationToken)
        {
            return _linkRepository.GetObjectId(request.Value, request.LinkType);
        }
    }
}
