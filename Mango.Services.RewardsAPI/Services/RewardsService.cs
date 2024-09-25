using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardAPI.Services
{
    public class RewardsService : IRewardsService
    {
        private DbContextOptions<AppDbContext> _appDbContext;
        public RewardsService(DbContextOptions<AppDbContext>? dbContext)
        {
            _appDbContext = dbContext;
        }

        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {

                await using var _db = new AppDbContext(_appDbContext);

                //// Get the UserId from the Rewards table
                //var userId = await _db.Rewards
                //    .Where(r => r.OrderId == rewardsMessage.OrderId)
                //    .Select(r => r.UserId)
                //    .FirstOrDefaultAsync();

                //if (userId != null && Convert.ToInt32(userId) > 0)
                //{
                //    rewardsMessage.UserEmail = await _db.Cart
                //        .Where(c => c.CartHeader.UserId == userId)
                //        .Select(c => c.CartHeader.Email)
                //        .FirstOrDefaultAsync() ?? string.Empty;
                //}

                Rewards rewards = new Rewards()
                {
                    OrderId = rewardsMessage.OrderId,
                    RewardsActivity = rewardsMessage.RewardsActivity,
                    UserId = rewardsMessage.UserId,
                    RewardsDate = DateTime.Now
                };

                await _db.Rewards.AddAsync(rewards);

                await _db.SaveChangesAsync();
                
            }
            catch (Exception)
            {
            }
        }
    }
}
