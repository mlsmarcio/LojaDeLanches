using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")] // Autenticado e com o perfil de admin
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
