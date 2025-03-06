using Api_EbayStocks.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api_EbayStocks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EbayStockController : ControllerBase
    {
        private readonly EbayStockServicios _stockServicios;

        public EbayStockController(EbayStockServicios stockServicios)
        {
            _stockServicios = stockServicios;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosLosStocks([FromQuery] int limit = 500)
        {
            var stocks = await _stockServicios.ObtenerTodosLosStocksAsync(limit);
            return Ok(stocks);
        }

        
        [HttpGet("informe-tendencias")]
        public async Task<IActionResult> GenerarInformeTendencias()
        {
            var excelFile = await _stockServicios.GenerarInformeTendenciasAsync();
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Informe_Tendencias.xlsx");
        }
        [HttpGet("informe-volumen")]
        public async Task<IActionResult> GenerarInformeVolumen()
        {
            var excelFile = await _stockServicios.GenerarInformeVolumenTransaccionesAsync();
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Informe_Volumen.xlsx");
        }

        [HttpGet("mapa-evolutivo-json")]
        public async Task<IActionResult> ObtenerMapaEvolutivoJson()
        {
            var json = await _stockServicios.ObtenerMapaEvolutivoJsonAsync();
            return Content(json, "application/json");
        }
    }
}
