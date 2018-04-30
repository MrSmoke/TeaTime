namespace TeaTime.Common.Features.Rooms
{
    using AutoMapper;
    using Commands;
    using Events;
    using Models.Data;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateRoomCommand, Room>()
                .ForMember(m => m.CreatedBy, cfg => cfg.MapFrom(src => src.UserId));

            CreateMap<CreateRoomCommand, RoomCreatedEvent>();
        }
    }
}
