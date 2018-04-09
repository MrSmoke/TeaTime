namespace TeaTime.Common.Features.Orders
{
    using AutoMapper;
    using Commands;
    using Events;
    using Models.Data;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateOrderCommand, Order>();
            CreateMap<Order, OrderPlacedEvent>()
                .ForMember(x => x.OrderId, o => o.MapFrom(x => x.Id));

            CreateMap<UpdateOrderOptionCommand, OrderOptionChangedEvent>();
        }
    }
}
