using Mango.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailAPI.Data
{
    // DBContext must be implemented when working with EFCore
    public class AppDbContext: DbContext
    {
        // Pass options to the base class and is required
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        // DBSet will be used as tableName in DB
        // Don't forget to add annotation to the object Model. Here, it is Email model.
        public DbSet<EmailLogger> EmailLoggers { get; set; }

        // OnModelCreating can be used to seed table
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
