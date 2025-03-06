using Api_EbayStocks.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Api_EbayStocks.data.Infraestructura
{
    public class AppDbContext : DbContext
    {
        public DbSet<EbayStock> Ebay_Stocks_Data { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=data/Infraestructura/ebay.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EbayStock>().HasNoKey();
        }
    }
}
