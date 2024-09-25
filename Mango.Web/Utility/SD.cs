namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }
        public static string CartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }

        public enum Roles
        {
            ADMIN,
            CUSTOMER
        };

        public const string TokenCookie = "JwtToken";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        };

        public enum OrderStatus
        {
            Status_Pending,
            Status_Approved,
            Status_ReadyForPickUp,
            Status_Completed,
            Status_Refunded,
            Status_Cancelled
        };
    }
}
