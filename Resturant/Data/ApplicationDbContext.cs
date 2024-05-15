using Microsoft.EntityFrameworkCore;
using Resturant.Models;

namespace Resturant.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }// object
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Report> Report { get; set; }
    }
}
