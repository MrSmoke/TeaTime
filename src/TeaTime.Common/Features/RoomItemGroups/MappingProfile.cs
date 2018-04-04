namespace TeaTime.Common.Features.RoomItemGroups
{
    using AutoMapper;
    using Commands;
    using Common.Models.Data;
    using Models;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateRoomItemGroupCommand, RoomItemGroup>();
            CreateMap<RoomItemGroup, RoomItemGroupModel>();
        }
    }
}
