using Microsoft.AspNetCore.Mvc;

namespace MTGRoyal.Controllers
{
    public class CartasController : Controller
    {
        public IActionResult Detalle(string id)
        {
            ViewBag.CardId = id;

            return View();
        }
    }
}