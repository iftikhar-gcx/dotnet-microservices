using Mango.Services.RewardAPI.Models;
using Mango.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.RewardAPI.Data
{
    // DBContext must be implemented when working with EFCore
    public class AppDbContext: DbContext
    {
        // Pass options to the base class and is required
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public DbSet<Rewards> Rewards { get; set; }
    }
}
