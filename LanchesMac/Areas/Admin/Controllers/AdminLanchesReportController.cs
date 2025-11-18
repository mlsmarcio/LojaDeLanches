using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using LanchesMac.Areas.Admin.FastReportUtils;
using LanchesMac.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminLanchesReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly RelatorioLanchesService _relatorioLanchesService;

        public AdminLanchesReportController(IWebHostEnvironment webHostEnv, RelatorioLanchesService relatorioLanchesService)
        {
            _webHostEnv = webHostEnv;
            _relatorioLanchesService = relatorioLanchesService;
        }

        public async Task<ActionResult> LanchesCategoriaReport()
        {
            var webReport = new WebReport();
            var mssqlDataConnection = new MsSqlDataConnection();

            // Registra os objetos
            webReport.Report.Dictionary.AddChild(mssqlDataConnection);
            // Carregar a instancia do relatório com o path completo do arquivo frx
            webReport.Report.Load(Path.Combine(_webHostEnv.ContentRootPath, "wwwroot/reports", "lanchesCategoria.frx"));
            // Gerar os datatables
            var lanches = HelperFastReport.GetTable(await _relatorioLanchesService.GetLanchesReport(), "LanchesReport");
            var categorias = HelperFastReport.GetTable(await _relatorioLanchesService.GetCategoriasReport(), "CategoriasReport");
            // Registrar os datatables para poder usar
            webReport.Report.RegisterData(lanches, "LancheReport");
            webReport.Report.RegisterData(categorias, "CategoriasReport");

            return View(webReport);
        }
        public async Task<ActionResult> LanchesCategoriaPDF()
        {
            var webReport = new WebReport();
            var mssqlDataConnection = new MsSqlDataConnection();

            // Registra os objetos
            webReport.Report.Dictionary.AddChild(mssqlDataConnection);
            // Carregar a instancia do relatório com o path completo do arquivo frx
            webReport.Report.Load(Path.Combine(_webHostEnv.ContentRootPath, "wwwroot/reports", "lanchesCategoria.frx"));
            // Gerar os datatables
            var lanches = HelperFastReport.GetTable(await _relatorioLanchesService.GetLanchesReport(), "LanchesReport");
            var categorias = HelperFastReport.GetTable(await _relatorioLanchesService.GetCategoriasReport(), "CategoriasReport");
            // Registrar os datatables para poder usar
            webReport.Report.RegisterData(lanches, "LancheReport");
            webReport.Report.RegisterData(categorias, "CategoriasReport");

            // Gerando o PDF
            webReport.Report.Prepare();
            Stream stream = new MemoryStream();
            webReport.Report.Export(new PDFSimpleExport(), stream);
            stream.Position = 0;
            // Faz o download
            //return File(stream, "application/zip", "LanchesCategoria.pdf");

            // Abre no navegador
            return new FileStreamResult(stream, "application/pdf");

        }
    }
}
