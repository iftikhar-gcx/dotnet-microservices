using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(ConfigurationBinder =>
            {
                ConfigurationBinder.CreateMap<OrderHeaderDTO, CartHeaderDTO>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();

                ConfigurationBinder.CreateMap<CartDetailsDTO, OrderDetailsDTO>()
                    .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.ProductName))
                    .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

                ConfigurationBinder.CreateMap<OrderDetailsDTO, CartDetailsDTO>();

                ConfigurationBinder.CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
                ConfigurationBinder.CreateMap<OrderDetails, OrderDetailsDTO>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
