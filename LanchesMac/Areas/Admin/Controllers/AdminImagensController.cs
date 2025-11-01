using LanchesMac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminImagensController : Controller
    {
        private readonly ConfigurationImagens _myConfig;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminImagensController(IWebHostEnvironment hostingEnvironment, IOptions<ConfigurationImagens> myConfig)
        {
            // Utilizada para obter caminho físico da minha pasta wwwroot do projeto e do caminho completo do arquivo
            _hostingEnvironment = hostingEnvironment;
            // obter o valor do arquivo de configuração
            _myConfig = myConfig.Value;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
