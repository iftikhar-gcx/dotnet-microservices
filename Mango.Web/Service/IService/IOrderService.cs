using Mango.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDTO?> CreateOrder(CartDTO cartDto);
        Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequestDto);
        Task<ResponseDTO?> ValidateStripeSession(int orderHeaderId);
        Task<ResponseDTO?> GetAllOrders(string? uderId);
        Task<ResponseDTO?> GetOrder(int orderId);
        Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus);
    }
}
