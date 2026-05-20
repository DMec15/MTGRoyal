using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTGRoyal.Models;

namespace MTGRoyal.Controllers
{

    public class CatalogoController : Controller
    {

        private readonly MtgroyalDbContext _db;

        public CatalogoController(MtgroyalDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var cartas = await _db.Cartas
                .Include(c => c.Colors)
                .ToListAsync();

            return View(cartas);
        }
    }
}
