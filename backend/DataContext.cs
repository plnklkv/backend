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
    }
}
