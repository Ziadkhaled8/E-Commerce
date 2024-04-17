using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }
}
