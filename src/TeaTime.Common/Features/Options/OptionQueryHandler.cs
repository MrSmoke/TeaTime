namespace TeaTime.Common.Features.Options
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using MediatR;
    using Models.Data;

    public class OptionQueryHandler : IRequestHandler<GetOptionQuery, Option>
    {
        private readonly IOptionsRepository _optionsRepository;

        public OptionQueryHandler(IOptionsRepository optionsRepository)
        {
            _optionsRepository = optionsRepository;
        }

        public Task<Option> Handle(GetOptionQuery request, CancellationToken cancellationToken)
        {
            return _optionsRepository.GetAsync(request.Id);
        }
    }
}
