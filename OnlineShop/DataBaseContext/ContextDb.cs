using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.DataBaseContext
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions options) : base(options) 
        {
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrdersStatus { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique(); // уникальность email
            modelBuilder.Entity<Login>().HasIndex(l => l.Login1).IsUnique(); // уникальность login
            modelBuilder.Entity<Basket>().HasIndex(b => b.IdUser).IsUnique(); // уникальность в basket, 
        }
    }
}
