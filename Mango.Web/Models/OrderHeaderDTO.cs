namespace Mango.Web.Models
{
    public class OrderHeaderDTO
    {
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double OrderTotal { get; set; }

        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public DateTime? OrderTime { get; set; } = DateTime.Now;
        public string? Status { get; set; } = string.Empty;
        public string? StripeSessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public IEnumerable<OrderDetailsDTO>? OrderDetails { get; set; }
    }
}
