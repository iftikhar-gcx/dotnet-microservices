using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace Mango.Services.EmailAPI.Models
{
    public class EmailLogger
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? EmailSent { get; set; } = DateTime.UtcNow;
    }
}
