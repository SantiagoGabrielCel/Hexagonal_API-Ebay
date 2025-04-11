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
            var baseDir = AppContext.BaseDirectory;
            var currentDir = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(baseDir);
            var rootFiles = Directory.GetFiles("/app");

            var dbPath = Path.Combine(baseDir, "Ebay.db");

            Console.WriteLine("========== DEBUG DB PATH INFO ==========");
            Console.WriteLine($"[BASE DIR]         AppContext.BaseDirectory: {baseDir}");
            Console.WriteLine($"[CURRENT DIR]      Directory.GetCurrentDirectory(): {currentDir}");
            Console.WriteLine($"[DB PATH]          Ruta esperada de DB: {dbPath}");
            Console.WriteLine($"[EXISTS]           ¿Existe el archivo? {File.Exists(dbPath)}");

            Console.WriteLine("\n[ARCHIVOS en BASE DIR]:");
            foreach (var f in files)
                Console.WriteLine($"  -> {f}");

            Console.WriteLine("\n[ARCHIVOS en /app]:");
            foreach (var f in rootFiles)
                Console.WriteLine($"  -> {f}");

            Console.WriteLine("========================================");

            if (!File.Exists(dbPath))
                throw new FileNotFoundException("La base de datos no se encuentra donde se espera.");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EbayStock>().HasNoKey();
        }
    }
}
