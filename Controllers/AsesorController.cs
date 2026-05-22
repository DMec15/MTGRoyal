using Microsoft.AspNetCore.Mvc;
using MTGRoyal.Services;

namespace MTGRoyal.Controllers
{
    public class AsesorController : Controller
    {
        private readonly IAService servicioAsesor;
        private readonly TemporaryStateService temporaryState;

        public AsesorController(
            IAService servicioAsesor,
            TemporaryStateService temporaryState)
        {
            this.servicioAsesor = servicioAsesor;
            this.temporaryState = temporaryState;
        }

        public IActionResult Index()
        {
            ViewBag.ChatMessages = temporaryState.GetAdvisorMessages();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                ViewBag.ChatMessages = temporaryState.GetAdvisorMessages();
                return View();
            }

            var respuesta = await servicioAsesor.GenerarTexto(prompt);

            temporaryState.AddAdvisorExchange(prompt, respuesta);
            ViewBag.ChatMessages = temporaryState.GetAdvisorMessages();

            return View();
        }
    }
}
