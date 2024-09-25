using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        // To invoke the Coupon Service API
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        // Default is Index() but renamed to CouponIndex as in _Layout.cshtml  asp-action
        // Also, default return type is IActionResult but since we are invoking async method,
        // we need to use async and Task<IActionResult> to support await keyword
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO>? list = new();
            ResponseDTO? response = await _couponService.GetAllCouponsAsync();

            if (response != null && response.isSuccess && response.Result != null)
            {
                // Deserialize the result object, but before that convert it string for conversion
                string? resultStr = response.Result.ToString();
                list = JsonConvert.DeserializeObject<List<CouponDTO>>(resultStr);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            // To show the list in view, add a @model in view
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDTO couponDTO)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _couponService.CreateCouponAsync(couponDTO);

                if (response != null && response.isSuccess)
                {
                    TempData["success"] = "Coupon created successfully!";
                    // nameof(CouponIndex) is same as "CouponIndex" magic string.
                    return RedirectToAction(nameof(CouponIndex));

                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(couponDTO);
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseDTO? response = await _couponService.GetCouponByIdAsync(couponId);

            if (response != null && response.isSuccess)
            {

                // Deserialize the result object, but before that convert it string for conversion
                string? resultStr = response.Result.ToString();
                CouponDTO model = JsonConvert.DeserializeObject<CouponDTO>(resultStr);

                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
		public async Task<IActionResult> CouponDelete(CouponDTO couponDTO)
		{
			ResponseDTO? response = await _couponService.DeleteCouponAsync(couponDTO.CouponId);

			if (response != null && response.isSuccess)
			{
                TempData["success"] = "Coupon deleted successfully!";

                return RedirectToAction(nameof(CouponIndex));
			}
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(couponDTO);
		}
	}
}
