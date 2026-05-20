using Microsoft.AspNetCore.Mvc;

namespace MTGRoyal.Controllers
{
    public class TiendasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}