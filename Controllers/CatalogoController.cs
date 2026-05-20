using Microsoft.AspNetCore.Mvc;

namespace MTGRoyal.Controllers
{
    public class CatalogoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}