using Microsoft.EntityFrameworkCore;
using OrderService.Data.Entities;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace OrderService.Data
{
    /// <summary>
    ///     Class inheriting DbContext to provide
    ///     access to the DB
    /// </summary>
    public class OrderDBContext(DbContextOptions<OrderDBContext> options) : DbContext(options)
    {
        public required DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Convert the table and all columns to lowercase as we are using PostgreSQL
            modelBuilder.Entity<Order>().ToTable("orders");
            modelBuilder.Entity<Order>().Property(o => o.Id).HasColumnName("id");
            modelBuilder.Entity<Order>().Property(o => o.UserId).HasColumnName("userid");
            modelBuilder.Entity<Order>().Property(o => o.Ticker).HasColumnName("ticker");
            modelBuilder.Entity<Order>().Property(o => o.Quantity).HasColumnName("quantity");
            modelBuilder.Entity<Order>().Property(o => o.Side).HasColumnName("side");
            modelBuilder.Entity<Order>().Property(o => o.Price).HasColumnName("price");
        }
    }
}
