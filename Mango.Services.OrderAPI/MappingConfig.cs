using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;


namespace Mango.Services.OrderAPI
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(o => o.CartTotal, u => u.MapFrom(scr => scr.OrderTotal)).ReverseMap();
            CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(scr => scr.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(scr => scr.Product.Price));
            CreateMap<OrderDetailsDto, CartDetailsDto>();
            CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
            CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();
        }
    }
}
