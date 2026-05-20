using Microsoft.AspNetCore.Mvc;

namespace MTGRoyal.Controllers
{
    public class AsesorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}