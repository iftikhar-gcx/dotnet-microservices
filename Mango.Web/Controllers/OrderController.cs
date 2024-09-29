using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using static Mango.Web.Utility.SD;

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

        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDTO orderHeaderDTO = new OrderHeaderDTO();
            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;

            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.isSuccess)
            {
                orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(response.Result.ToString());
            }

            if(!User.IsInRole(SD.Roles.ADMIN.ToString()) && userId != orderHeaderDTO.UserId)
            {
                return NotFound();
            }

            return View(orderHeaderDTO);
        }

        [HttpGet]
        public IActionResult GetAll(string status)
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

                OrderStatus orderStatus;
                Enum.TryParse(status, out orderStatus);

                switch (orderStatus)
                {
                    case OrderStatus.Status_Pending:
                        ordersData = ordersData.Where(u => u.Status == OrderStatus.Status_Pending.ToString());
                        break;
                    case OrderStatus.Status_Approved:
                        ordersData = ordersData.Where(u => u.Status == OrderStatus.Status_Approved.ToString());
                        break;
                    case OrderStatus.Status_ReadyForPickUp:
                        ordersData = ordersData.Where(u => u.Status == OrderStatus.Status_ReadyForPickUp.ToString());
                        break;
                    case OrderStatus.Status_Completed:
                        ordersData = ordersData.Where(u => u.Status == OrderStatus.Status_Completed.ToString());
                        break;
                    case OrderStatus.Status_Refunded:
                        ordersData = ordersData.Where(u => u.Status == OrderStatus.Status_Refunded.ToString());
                        break;
                    case OrderStatus.Status_Cancelled:
                        ordersData = ordersData.Where(u => u.Status == OrderStatus.Status_Cancelled.ToString());
                        break;
                    default:
                        break;
                }
            }
            else
            { 
                ordersData = new List<OrderHeaderDTO>();
            }

			return Json(new { data = ordersData });
		}

        [HttpPost]
        public async Task<IActionResult> OrderReadyForPickUp(int orderId)
        {
            var orderStatus = await _orderService.UpdateOrderStatus(orderId, OrderStatus.Status_ReadyForPickUp.ToString());
            if (orderStatus != null && orderStatus.isSuccess)
            {
                TempData["success"] = "Status updated successfully!";
                return RedirectToAction(nameof(OrderIndex), new { orderId });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var orderStatus = await _orderService.UpdateOrderStatus(orderId, OrderStatus.Status_Completed.ToString());
            if (orderStatus != null && orderStatus.isSuccess)
            {
                TempData["success"] = "Status completed successfully!";
                return RedirectToAction(nameof(OrderIndex), new { orderId });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var orderStatus = await _orderService.UpdateOrderStatus(orderId, OrderStatus.Status_Cancelled.ToString());
            if (orderStatus != null && orderStatus.isSuccess)
            {
                TempData["success"] = "Status cencelled successfully!";
                return RedirectToAction(nameof(OrderIndex), new { orderId });
            }

            return View();
        }
    }
}
