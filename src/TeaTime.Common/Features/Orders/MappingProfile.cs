namespace TeaTime.Common.Features.Orders
{
    using AutoMapper;
    using Commands;
    using Models.Data;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateOrderCommand, Order>();
        }
    }
}
