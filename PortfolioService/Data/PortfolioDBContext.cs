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

            // Configure Portfolio to be keyless
            modelBuilder.Entity<Portfolio>().HasNoKey();
        }
    }
}
