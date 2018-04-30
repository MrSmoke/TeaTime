namespace TeaTime.Common.Features.Users
{
    using AutoMapper;
    using Commands;
    using Models.Data;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserCommand, User>();
        }
    }
}
