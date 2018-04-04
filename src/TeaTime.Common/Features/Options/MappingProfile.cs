namespace TeaTime.Common.Features.Options
{
    using AutoMapper;
    using Commands;
    using Models.Data;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateOptionCommand, Option>()
                .ForMember(o => o.CreatedBy, o => o.MapFrom(c => c.UserId));
        }
    }
}
