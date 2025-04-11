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
            var dbPath = Path.Combine(AppContext.BaseDirectory, "ebay.db");

            Console.WriteLine($"[DEBUG] Ruta esperada de DB: {dbPath}");
            Console.WriteLine($"[DEBUG] ¿Existe el archivo? {File.Exists(dbPath)}");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EbayStock>().HasNoKey();
        }
    }
}
