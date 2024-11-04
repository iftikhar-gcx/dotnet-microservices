using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        // To invoke the Base service
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService) 
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> CreateOrder(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = OrderAPIBase + "/api/order/create-order"
            });
        }

        public async Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequestDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = stripeRequestDto,
                Url = OrderAPIBase + "/api/order/create-stripe-session"
            });
        }

        public async Task<ResponseDTO?> GetAllOrders(string? userId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = OrderAPIBase + "/api/order/get-orders/" + userId
            });
        }

        public async Task<ResponseDTO?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = OrderAPIBase + "/api/order/get-order/" + orderId
            });
        }

        public async Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = newStatus,
                Url = OrderAPIBase + "/api/order/update-order-status/" + orderId
            });
        }

        public async Task<ResponseDTO?> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = orderHeaderId,
                Url = OrderAPIBase + "/api/order/validate-stripe-session"
            });
        }
    }
}
