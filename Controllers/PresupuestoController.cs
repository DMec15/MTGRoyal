using Microsoft.AspNetCore.Mvc;

namespace MTGRoyal.Controllers
{
    public class PresupuestoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}