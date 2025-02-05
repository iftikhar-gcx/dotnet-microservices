using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class CartService : ICartService
    {
        // To invoke the Base service
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService) 
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> GetCartByIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = CartAPIBase + "/api/cart/get-cart/" + userId
            });
        }

        public async Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = CartAPIBase + "/api/cart/cart-upsert"
            });
        }

        public async Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDetailsId,
                Url = CartAPIBase + "/api/cart/cart-remove"
            });
        }

        public async Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = CartAPIBase + "/api/cart/apply-coupon"
            });
        }

        public async Task<ResponseDTO?> EmailCartAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = CartAPIBase + "/api/cart/email-cart-request"
            });
        }
    }
}
