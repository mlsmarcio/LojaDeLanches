using LanchesMac.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Areas.Admin.Controllers
{
    public class AdminGraficoController : Controller
    {
        public readonly GraficoVendasService _graficoVendas;

        public AdminGraficoController(GraficoVendasService graficoVendas)
        {
            _graficoVendas = graficoVendas ?? throw new ArgumentNullException(nameof(graficoVendas));
        }

        // MÉTODO PARA OBTER O RESULTADO DA CONSULTA UTILIZANDO A CLASE SERVICE
        public JsonResult VendasLanches(int dias) {
            var lanchesVendasTotais = _graficoVendas.GetVendasLanches(dias);
            return Json(lanchesVendasTotais);
        }

        // VENDAS DOS ÚLTIMOS 360 DIAS
        [HttpGet]
        public IActionResult Index(int dias)
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult VendasMensais(int dias)
        {
            return View();
        }

        [HttpGet]
        public IActionResult VendasSemanal(int dias)
        {
            return View();
        }
    }
}
