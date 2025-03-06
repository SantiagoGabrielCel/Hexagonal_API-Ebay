using Api_EbayStocks.Core.Entities;
using Api_EbayStocks.Core.Interface;

namespace Api_EbayStocks.Application.Services
{
    public class EbayStockServicios
    {
        private readonly IEbayStockRepositorio _stockRepositorio;

        public EbayStockServicios(IEbayStockRepositorio stockRepositorio)
        {
            _stockRepositorio = stockRepositorio;
        }

        public async Task<List<EbayStock>> ObtenerTodosLosStocksAsync(int limit)
        {
            return await _stockRepositorio.ObtenerTodosLosStocksAsync(limit);
        }

        public async Task<EbayStock> ObtenerStockPorFechaAsync(string fecha)
        {
            return await _stockRepositorio.ObtenerStockPorFechaAsync(fecha);
        }
        public async Task<byte[]> GenerarInformeTendenciasAsync()
        {
            return await _stockRepositorio.GenerarInformeTendenciasAsync();
        }
        public async Task<byte[]> GenerarInformeVolumenTransaccionesAsync()
        {
            return await _stockRepositorio.GenerarInformeVolumenTransaccionesAsync();
        }
        public async Task<string> ObtenerMapaEvolutivoJsonAsync()
        {
            return await _stockRepositorio.ObtenerMapaEvolutivoJsonAsync();
        }
        
    }
}