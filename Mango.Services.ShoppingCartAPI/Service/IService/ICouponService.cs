﻿using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDTO> GetCoupons(string couponCode);
    }
}
