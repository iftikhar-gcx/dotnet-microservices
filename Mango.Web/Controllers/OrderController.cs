using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult OrderIndex()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeaderDTO> ordersData;
            string userId = string.Empty;

            if(!User.IsInRole(SD.Roles.ADMIN.ToString()))
            {
                userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value ?? string.Empty;
            }

            ResponseDTO responseDTO = _orderService.GetAllOrders(userId).GetAwaiter().GetResult() ?? new() { };
            if(responseDTO != null && responseDTO.isSuccess)
            {
                ordersData = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDTO>>(responseDTO.Result.ToString()) ?? new List<OrderHeaderDTO>();
            }
            else
            { 
                ordersData = new List<OrderHeaderDTO>();
            }

			return Json(new { data = ordersData });
		}
    }
}
