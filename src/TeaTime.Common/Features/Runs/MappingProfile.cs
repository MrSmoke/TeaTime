namespace TeaTime.Common.Features.Runs
{
    using AutoMapper;
    using Commands;
    using Models.Data;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StartRunCommand, Run>()
                .ForMember(m => m.GroupId, o => o.MapFrom(m => m.RoomGroupId));
        }
    }
}
