﻿using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(ConfigurationBinder =>
            {
                ConfigurationBinder.CreateMap<ProductDTO, Product>();
                ConfigurationBinder.CreateMap<Product, ProductDTO>();
            });

            return mappingConfig;
        }
    }
}
