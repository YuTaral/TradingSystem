using Microsoft.EntityFrameworkCore;
using PortfolioService.Data.Entities;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace PortfolioService.Data
{
    /// <summary>
    ///     Class inheriting DbContext to provide
    ///     access to the DB
    /// </summary>
    public class PortfolioDBContext(DbContextOptions<PortfolioDBContext> options) : DbContext(options)
    {
        public required DbSet<Portfolio> Portfolios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Convert the table and all columns to lowercase as we are using PostgreSQL
            modelBuilder.Entity<Portfolio>().ToTable("portfolios");
            modelBuilder.Entity<Portfolio>().Property(p => p.Id).HasColumnName("id");
            modelBuilder.Entity<Portfolio>().Property(p => p.UserId).HasColumnName("userid");
            modelBuilder.Entity<Portfolio>().Property(p => p.Ticker).HasColumnName("ticker");
            modelBuilder.Entity<Portfolio>().Property(p => p.Quantity).HasColumnName("quantity");

            // Add UserId index
            modelBuilder.Entity<Portfolio>().HasIndex(p => p.UserId);
        }
    }
}
