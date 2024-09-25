using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        // To invoke the Base service
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService) 
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = CouponAPIBase + "/api/coupon"
            });
        }

        public async Task<ResponseDTO?> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = CouponAPIBase + "/api/coupon/GetByCode/" + couponCode
            });
        }

        public async Task<ResponseDTO?> GetCouponByIdAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = CouponAPIBase + "/api/coupon/" + couponId
            });
        }

        public async Task<ResponseDTO?> CreateCouponAsync(CouponDTO couponDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = couponDto,
                Url = CouponAPIBase + "/api/coupon/"
            });
        }

        public async Task<ResponseDTO?> UpdateCouponAsync(CouponDTO couponDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.PUT,
                Data = couponDto,
                Url = CouponAPIBase + "/api/coupon/"
            });
        }

        public async Task<ResponseDTO?> DeleteCouponAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.DELETE,
                Url = CouponAPIBase + "/api/coupon/" + couponId
            });
        }
    }
}
