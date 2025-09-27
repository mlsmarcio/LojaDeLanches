using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers
{
    // [AllowAnonymous] PODE SER USADO PARA ACESSO NÃO AUTENTICADO EM QUALQUER MÉTODO. TÊM PRECENDÊNCIA SOBRE AS OUTRAS DIRETIVAS
    // [Authorize(Roles ="Admin")] VERIFICA SE O USUÁRIO ESTÁ AUTENTICADO E SE PERTENCE AO PERFIL Admin
    [Authorize] // TODOS OS MÉTODOS VERIFICARÃO SE O USUÁRIO ESTÁ AUTENTICADO. 
    public class ContatoController : Controller
    {
        public IActionResult Index()
        {
            //// FORMA DE VERIFICAR SE O USUÁRIO ESTÁ AUTENTICADO
            //return User.Identity.IsAuthenticated ?  View() : RedirectToAction("Login", "Account");
            return View();
        }
    }
}
