using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(ConfigurationBinder =>
            {
                ConfigurationBinder.CreateMap<CartHeader, CartHeaderDTO>();
                ConfigurationBinder.CreateMap<CartHeaderDTO, CartHeader>();


                ConfigurationBinder.CreateMap<CartDetails, CartDetailsDTO>();
                ConfigurationBinder.CreateMap<CartDetailsDTO, CartDetails>();
            });

            return mappingConfig;
        }
    }
}
