namespace TeaTime.Common.Features.Runs
{
    using AutoMapper;
    using Commands;
    using Events;
    using Models.Data;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StartRunCommand, Run>()
                .ForMember(m => m.GroupId, o => o.MapFrom(m => m.RoomGroupId));
            CreateMap<Run, RunStartedEvent>();
        }
    }
}
