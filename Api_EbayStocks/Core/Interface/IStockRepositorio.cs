using Api_EbayStocks.Core.Entities;
using System.Collections.Generic;

namespace Api_EbayStocks.Core.Interface
{
    public interface IEbayStockRepositorio
    {
        Task<List<EbayStock>> ObtenerTodosLosStocksAsync(int limit);
        Task<EbayStock> ObtenerStockPorFechaAsync(string fecha);

        Task<byte[]> GenerarInformeTendenciasAsync();
        Task<byte[]> GenerarInformeVolumenTransaccionesAsync();
        Task<string> ObtenerMapaEvolutivoJsonAsync();
    }
}
