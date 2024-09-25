using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.RewardAPI.Models
{
    public class Rewards
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime RewardsDate { get; set; } = DateTime.Now;
        public int RewardsActivity { get; set; }
        public int OrderId { get; set; }

    }
}
