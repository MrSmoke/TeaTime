namespace TeaTime.Common.Features.RoomItemGroups
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Data;
    using AutoMapper;
    using Common.Models.Data;
    using MediatR;
    using Models;
    using Queries;

    public class RoomItemGroupQueryHandler :
        IRequestHandler<GetRoomItemGroupByNameQuery, RoomItemGroupModel?>,
        IRequestHandler<GetRoomItemGroupQuery, RoomItemGroupModel?>
    {
        private readonly IOptionsRepository _optionsRepository;
        private readonly IMapper _mapper;

        public RoomItemGroupQueryHandler(IOptionsRepository optionsRepository, IMapper mapper)
        {
            _optionsRepository = optionsRepository;
            _mapper = mapper;
        }

        public async Task<RoomItemGroupModel?> Handle(GetRoomItemGroupByNameQuery request, CancellationToken cancellationToken)
        {
            var group = await _optionsRepository.GetGroupByNameAsync(request.RoomId, request.Name);
            if (group == null)
                return null;

            var model = _mapper.Map<RoomItemGroup, RoomItemGroupModel>(group);

            model.Options = (await _optionsRepository.GetOptionsByGroupIdAsync(model.Id)).ToList();

            return model;
        }

        public async Task<RoomItemGroupModel?> Handle(GetRoomItemGroupQuery request, CancellationToken cancellationToken)
        {
            var group = await _optionsRepository.GetGroupAsync(request.GroupId);
            if (group == null)
                return null;

            var model = _mapper.Map<RoomItemGroup, RoomItemGroupModel>(group);

            model.Options = (await _optionsRepository.GetOptionsByGroupIdAsync(model.Id)).ToList();

            return model;
        }
    }
}
