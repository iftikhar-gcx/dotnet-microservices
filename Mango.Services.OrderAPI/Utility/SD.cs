namespace Mango.Services.OrderAPI.Utility
{
    public class SD
    {
        public enum OrderStatus
        {
            Status_Pending,
            Status_Approved,
            Status_ReadyForPickUp,
            Status_Completed,
            Status_Refunded,
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
