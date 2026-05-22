using Microsoft.AspNetCore.Mvc;
using MTGRoyal.Services;

namespace MTGRoyal.Controllers
{
    public class AsesorController : Controller
    {
        private readonly IAService servicioAsesor;

        public AsesorController(IAService servicioAsesor)
        {
            this.servicioAsesor = servicioAsesor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string prompt)
        {
            ViewBag.Prompt = prompt;

            if (string.IsNullOrWhiteSpace(prompt))
                return View();

            var respuesta = await servicioAsesor.GenerarTexto(prompt);
            ViewBag.Texto = respuesta;
            return View();
        }
    }
}
