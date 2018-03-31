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
            CreateMap<StartRunCommand, Run>();
            CreateMap<Run, RunStartedEvent>();
        }
    }
}
