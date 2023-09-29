namespace TeaTime.Common.Features.Rooms
{
    using AutoMapper;
    using Commands;
    using Models.Data;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateRoomCommand, Room>()
                .ForMember(m => m.CreatedBy, cfg => cfg.MapFrom(src => src.UserId));
        }
    }
}
