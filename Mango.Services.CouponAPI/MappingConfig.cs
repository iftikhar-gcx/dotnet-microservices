using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(ConfigurationBinder =>
            {
                ConfigurationBinder.CreateMap<CouponDTO, Coupon>();
                ConfigurationBinder.CreateMap<Coupon, CouponDTO>();
            });

            return mappingConfig;
        }
    }
}
