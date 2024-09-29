using System.ComponentModel.DataAnnotations;

namespace Mango.Services.OrderAPI.Utility
{
    public class SD
    {
        public enum OrderStatus
        {
            [Display(Name = "Pending")]
            Status_Pending,
            [Display(Name = "Approved")]
            Status_Approved,
            [Display(Name = "Ready For PickUp")]
            Status_ReadyForPickUp,
            [Display(Name = "Completed")]
            Status_Completed,
            [Display(Name = "Refunded")]
            Status_Refunded,
            [Display(Name = "Cancelled")]
            Status_Cancelled
        };

        public enum Roles
        {
            ADMIN,
            CUSTOMER
        };

        public enum PaymentStatus
        {
            canceled,
            processing,
            requires_action,
            requires_capture,
            requires_confirmation,
            requires_payment_method,
            succeeded
        }
    }
}
