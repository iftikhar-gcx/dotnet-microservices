using System.ComponentModel.DataAnnotations;

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
            [Display(Name = "Admin")]
            ADMIN,
            [Display(Name = "Customer")]
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
            [Display(Name ="Pending")]
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

        public enum ContentType
        {
            Json,
            MultipartFormData
        }
    }
}
