using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Data
{
    // DBContext must be implemented when working with EFCore
    public class AppDbContext: DbContext
    {
        // Pass options to the base class and is required
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        // DBSet will be used as tableName in DB
        // Don't forget to add annotation to the object Model. Here, it is Coupon model.
        public DbSet<Coupon> Coupons { get; set; }

        // OnModelCreating can be used to seed table
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData( new Coupon
            { 
                CouponId = 1,
                CouponCode = "10OFF",
                DiscountAmount = 10,
                MinAmount = 20
            });

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 2,
                CouponCode = "20OFF",
                DiscountAmount = 20,
                MinAmount = 40
            });
        }
    }
}
