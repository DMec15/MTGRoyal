using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTGRoyal.Models;

namespace MTGRoyal.Controllers
{
    public class CartasController : Controller
    {
        private readonly MtgroyalDbContext _db;

        public CartasController(MtgroyalDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var carta = await _db.Cartas
                .Include(c => c.Rareza)
                .Include(c => c.Colors)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carta == null)
                return NotFound();

            return View(carta);
        }
    }
}
