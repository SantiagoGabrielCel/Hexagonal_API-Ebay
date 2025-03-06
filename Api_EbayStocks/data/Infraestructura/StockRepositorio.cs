using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Api_EbayStocks.Core.Entities;
using Api_EbayStocks.Core.Interface;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;


namespace Api_EbayStocks.data.Infraestructura
{
    public class EbayStockRepositorio : IEbayStockRepositorio
    {
        private readonly AppDbContext _dbContext;
        private static int _ultimoOffset = 0;
        public EbayStockRepositorio(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EbayStock>> ObtenerTodosLosStocksAsync(int limit = 500)
        {
            var stocks = await _dbContext.Ebay_Stocks_Data.AsNoTracking()
                .Skip(_ultimoOffset)
                .Take(limit)
                .ToListAsync();

            _ultimoOffset += stocks.Count; 
            return stocks;
        }

        public async Task<EbayStock> ObtenerStockPorFechaAsync(string fecha)
        {
            return await _dbContext.Ebay_Stocks_Data.FirstOrDefaultAsync(s => s.Date == fecha);
        }
        public async Task<byte[]> GenerarInformeTendenciasAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stocks = await _dbContext.Ebay_Stocks_Data.AsNoTracking().ToListAsync();
            if (!stocks.Any()) throw new Exception("No hay datos para generar el informe.");

            var precios = stocks.Select(s => s.Close).ToList();
            var fechas = stocks.Select(s => s.Date).ToList();

            double promedio = precios.Average();
            double mediana = CalcularMediana(precios);
            double moda = CalcularModa(precios);
            double maximo = precios.Max();
            double minimo = precios.Min();
            double desviacionEstandar = CalcularDesviacionEstandar(precios);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Tendencias");

            worksheet.Cells["A1"].Value = "Estadística";
            worksheet.Cells["B1"].Value = "Valor";
            worksheet.Cells["A2"].Value = "Promedio";
            worksheet.Cells["B2"].Value = promedio;
            worksheet.Cells["A3"].Value = "Mediana";
            worksheet.Cells["B3"].Value = mediana;
            worksheet.Cells["A4"].Value = "Moda";
            worksheet.Cells["B4"].Value = moda;
            worksheet.Cells["A5"].Value = "Máximo";
            worksheet.Cells["B5"].Value = maximo;
            worksheet.Cells["A6"].Value = "Mínimo";
            worksheet.Cells["B6"].Value = minimo;
            worksheet.Cells["A7"].Value = "Desviación Estándar";
            worksheet.Cells["B7"].Value = desviacionEstandar;

            worksheet.Cells["D1"].Value = "Fecha";
            worksheet.Cells["E1"].Value = "Precio de Cierre";
            for (int i = 0; i < fechas.Count; i++)
            {
                worksheet.Cells[$"D{i + 2}"].Value = fechas[i];
                worksheet.Cells[$"E{i + 2}"].Value = precios[i];
            }

            var chart = worksheet.Drawings.AddChart("GraficoTendencia", OfficeOpenXml.Drawing.Chart.eChartType.Line);
            chart.Title.Text = "Tendencia de Precios";
            chart.SetPosition(10, 0, 7, 0);
            chart.SetSize(800, 400);
            chart.Series.Add(worksheet.Cells[$"E2:E{fechas.Count + 1}"], worksheet.Cells[$"D2:D{fechas.Count + 1}"]);
            chart.XAxis.Title.Text = "Fecha";
            chart.YAxis.Title.Text = "Precio de Cierre";

            return package.GetAsByteArray();
        }

        private static double CalcularMediana(List<double> valores)
        {
            valores.Sort();
            int n = valores.Count;
            return n % 2 == 0 ? (valores[n / 2 - 1] + valores[n / 2]) / 2.0 : valores[n / 2];
        }

        private static double CalcularModa(List<double> valores)
        {
            return valores.GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key;
        }

        private static double CalcularDesviacionEstandar(List<double> valores)
        {
            double media = valores.Average();
            return Math.Sqrt(valores.Sum(v => Math.Pow(v - media, 2)) / valores.Count);
        }

        public async Task<byte[]> GenerarInformeVolumenTransaccionesAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stocks = await _dbContext.Ebay_Stocks_Data.AsNoTracking().ToListAsync();
            if (!stocks.Any()) throw new Exception("No hay datos disponibles para generar el informe.");

            var categorias = new Dictionary<string, int>
            {
                { "Muy Bajo (<500K)", stocks.Count(s => s.Volume < 500000) },
                { "Bajo (500K - 2M)", stocks.Count(s => s.Volume >= 500000 && s.Volume < 2000000) },
                { "Medio (2M - 5M)", stocks.Count(s => s.Volume >= 2000000 && s.Volume < 5000000) },
                { "Alto (5M - 10M)", stocks.Count(s => s.Volume >= 5000000 && s.Volume < 10000000) },
                { "Extremo (>10M)", stocks.Count(s => s.Volume >= 10000000) }
            };

            using var ExcelPackage_v = new ExcelPackage();
            var worksheet = ExcelPackage_v.Workbook.Worksheets.Add("Volumen Transacciones");

            worksheet.Cells["A1"].Value = "Categoría";
            worksheet.Cells["B1"].Value = "Cantidad de Transacciones";

            int fila = 2;
            foreach (var categoria in categorias)
            {
                worksheet.Cells[$"A{fila}"].Value = categoria.Key;
                worksheet.Cells[$"B{fila}"].Value = categoria.Value;
                fila++;
            }

            var chart = worksheet.Drawings.AddChart("GraficoTorta", OfficeOpenXml.Drawing.Chart.eChartType.Pie);
            chart.Title.Text = "Distribución del Volumen de Transacciones";
            chart.SetPosition(10, 0, 3, 0);
            chart.SetSize(600, 400);
            chart.Series.Add(worksheet.Cells[$"B2:B{fila - 1}"], worksheet.Cells[$"A2:A{fila - 1}"]);

            return ExcelPackage_v.GetAsByteArray();
        }
        public async Task<byte[]> GenerarInformeMapaEvolutivoAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stocks = await _dbContext.Ebay_Stocks_Data.AsNoTracking().ToListAsync();
            if (!stocks.Any()) return null; 

            var eventosImportantes = new Dictionary<string, string>
            {
                { "2010-09-15", "eBay anuncia PayPal como líder en pagos online" },
                { "2015-07-20", "PayPal se separa oficialmente de eBay" },
                { "2018-02-01", "Se reportan ingresos récord en eBay" },
                { "2021-03-15", "Boom del e-commerce tras la pandemia" },
                { "2022-09-01", "Caída del mercado tecnológico afecta a eBay" }
            };

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Mapa Evolutivo");

            worksheet.Cells["A1"].Value = "Fecha";
            worksheet.Cells["B1"].Value = "Precio de Cierre";
            worksheet.Cells["C1"].Value = "Evento";

            int row = 2;
            foreach (var stock in stocks)
            {
                worksheet.Cells[$"A{row}"].Value = stock.Date;
                worksheet.Cells[$"B{row}"].Value = stock.Close;
                worksheet.Cells[$"C{row}"].Value = eventosImportantes.ContainsKey(stock.Date) ? eventosImportantes[stock.Date] : "";
                row++;
            }

            var chart = worksheet.Drawings.AddChart("GraficoEvolutivo", OfficeOpenXml.Drawing.Chart.eChartType.Line);
            chart.Title.Text = "Evolución del Stock";
            chart.SetPosition(10, 0, 3, 0);
            chart.SetSize(800, 400);
            chart.Series.Add(worksheet.Cells[$"B2:B{row - 1}"], worksheet.Cells[$"A2:A{row - 1}"]);
            chart.XAxis.Title.Text = "Fecha";
            chart.YAxis.Title.Text = "Precio";

            return package.GetAsByteArray();
        }

        public async Task<string> ObtenerMapaEvolutivoJsonAsync()
        {
            var eventosImportantes = new Dictionary<DateTime, (string descripcion, string impacto, string opinion)>
            {
                { new DateTime(2010, 9, 15), ("eBay anuncia PayPal como líder en pagos online", "positivo", "Warren Buffett: 'Un movimiento estratégico inteligente para dominar los pagos digitales'.") },
                { new DateTime(2015, 7, 20), ("PayPal se separa oficialmente de eBay", "negativo", "Elon Musk: 'PayPal puede brillar por sí mismo, pero eBay pierde una ventaja clave'.") },
                { new DateTime(2018, 2, 1), ("Se reportan ingresos récord en eBay", "positivo", "Charlie Munger: 'Los ingresos récord son buenos, pero el valor real está en la retención de usuarios'.") },
                { new DateTime(2021, 3, 15), ("Boom del e-commerce tras la pandemia", "positivo", "Bill Gates: 'El comercio electrónico es el futuro, y eBay sigue siendo relevante'.") },
                { new DateTime(2022, 9, 1), ("Caída del mercado tecnológico afecta a eBay", "negativo", "Peter Lynch: 'Las caídas del mercado son oportunidades, pero eBay necesita innovación'.") }
            };

            var stocks = await _dbContext.Ebay_Stocks_Data.AsNoTracking().ToListAsync();

            var historial = stocks
                .Where(s => eventosImportantes.ContainsKey(s.FechaConvertida))
                .Select(s =>
                {
                    var fechaActual = s.FechaConvertida;
                    var stockAntes = stocks.FirstOrDefault(x => x.FechaConvertida < fechaActual)?.Close ?? s.Close;
                    //var cambioPorcentaje = ((s.Close - stockAntes) / stockAntes) * 100;

                    return new
                    {
                        fecha = s.FechaConvertida.ToString("yyyy-MM-dd"),
                        precio = s.Close,
                        //cambio_porcentaje = Math.Round(cambioPorcentaje, 2),
                        evento = eventosImportantes[s.FechaConvertida].descripcion,
                        impacto = eventosImportantes[s.FechaConvertida].impacto,
                        opinion = eventosImportantes[s.FechaConvertida].opinion
                    };
                })
                .ToList();

            if (!historial.Any()) return "{}";

            return JsonConvert.SerializeObject(new { historial }, Formatting.Indented);
        }


    }
}
   

