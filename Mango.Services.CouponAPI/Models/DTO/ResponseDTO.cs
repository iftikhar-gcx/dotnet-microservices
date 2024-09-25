namespace Mango.Services.CouponAPI.Models.DTO
{
    public class ResponseDTO
    {
        public object? Result { get; set; } = new object();
        public bool isSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;

    }
}
