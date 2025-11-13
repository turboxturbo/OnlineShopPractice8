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
    }
}
