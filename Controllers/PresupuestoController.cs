using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTGRoyal.Models;
using MTGRoyal.Services;

namespace MTGRoyal.Controllers
{
    public class PresupuestoController : Controller
    {

        private readonly MtgroyalDbContext _db;
        private readonly TemporaryStateService temporaryState;

        public PresupuestoController(
            MtgroyalDbContext db,
            TemporaryStateService temporaryState)
        {
            _db = db;
            this.temporaryState = temporaryState;
        }

        public async Task<IActionResult> Index()
        {

            var cartas = await _db.Cartas
                .Include(c => c.Rareza)
                .Include(c => c.Colors)
                .ToListAsync();

            ViewBag.BudgetState = temporaryState.GetBudgetState();

            return View(cartas);
        }

        [HttpPost]
        public IActionResult GuardarEstado([FromBody] BudgetState state)
        {
            temporaryState.SaveBudgetState(state);

            return Ok();
        }
    }
}
