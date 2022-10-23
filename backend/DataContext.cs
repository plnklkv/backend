global using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Product> Products => Set<Product>();

        public DbSet<Cart> Carts => Set<Cart>();

        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                SurnName = "SurnName",
                FirstName = "FirstName",
                MiddleName = "MiddleName",
                Email = "user@shop.ru",
                Password = "password",
                Role = "user"
            }, new User
            {
                Id = 2,
                SurnName = "SurnName",
                FirstName = "FirstName",
                MiddleName = "MiddleName",
                Email = "admin@shop.ru",
                Password = "QWEasd123",
                Role = "admin"
            });
        }
    }
}
