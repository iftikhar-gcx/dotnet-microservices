using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.EmailAPI.Message
{
    public class RewardsMessage
    {
        public string UserId { get; set; }
        [NotMapped]
        public string UserEmail { get; set; }
        public int RewardsActivity { get; set; }
        public int OrderId { get; set; }
    }
}
