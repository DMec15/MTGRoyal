using Microsoft.AspNetCore.Mvc;

namespace MTGRoyal.Controllers
{
    public class PersonalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}