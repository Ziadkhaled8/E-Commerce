using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
            CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
        }
    }
}
