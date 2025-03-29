using Microsoft.EntityFrameworkCore;
using OrderService.Data.Entities;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace OrderService.Data
{
    /// <summary>
    ///     Class inheriting DbContext to provide
    ///     access to the DB
    /// </summary>
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        public required DbSet<Order> Orders { get; set; }
    }
}
