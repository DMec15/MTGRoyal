using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTGRoyal.Models;

namespace MTGRoyal.Controllers
{
    public class PersonalController : Controller
    {
        private readonly MtgroyalDbContext _db;

        public PersonalController(MtgroyalDbContext db)
        {
            _db = db;
        }

        public class CartaCreateRequest
        {
            public string Nombre { get; set; } = string.Empty;

            public decimal Precio { get; set; }

            public int RarezaId { get; set; }

            public string Tipo { get; set; } = string.Empty;

            public string? Coleccion { get; set; }

            public string? ImagenUrl { get; set; }

            public int[] ColorIds { get; set; } = [];
        }

        public async Task<IActionResult> Index()
        {
            await LoadFormOptions();

            ViewBag.Cartas = await _db.Cartas
                .Include(c => c.Rareza)
                .Include(c => c.Colors)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CartaCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                ModelState.AddModelError(nameof(request.Nombre), "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Tipo))
                ModelState.AddModelError(nameof(request.Tipo), "El tipo es obligatorio.");

            if (request.RarezaId <= 0)
                ModelState.AddModelError(nameof(request.RarezaId), "La rareza es obligatoria.");

            if (request.Precio < 0)
                ModelState.AddModelError(nameof(request.Precio), "El precio no puede ser negativo.");

            if (ModelState.IsValid)
            {
                var rarezaExists = await _db.Rarezas
                    .AnyAsync(rareza => rareza.Id == request.RarezaId);

                if (!rarezaExists)
                    ModelState.AddModelError(nameof(request.RarezaId), "Selecciona una rareza valida.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Form = request;
                await LoadFormOptions();
                return View();
            }

            var colores = await _db.Colores
                .Where(color => request.ColorIds.Contains(color.Id))
                .ToListAsync();

            var carta = new Carta
            {
                Nombre = request.Nombre.Trim(),
                Precio = request.Precio,
                RarezaId = request.RarezaId,
                Tipo = request.Tipo.Trim(),
                Coleccion = request.Coleccion?.Trim(),
                ImagenUrl = request.ImagenUrl?.Trim()
            };

            foreach (var color in colores)
            {
                carta.Colors.Add(color);
            }

            _db.Cartas.Add(carta);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Carta agregada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadFormOptions()
        {
            ViewBag.Colores = await _db.Colores
                .OrderBy(color => color.Nombre)
                .ToListAsync();

            ViewBag.Rarezas = await _db.Rarezas
                .OrderBy(rareza => rareza.Nombre)
                .ToListAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            string nombre,
            decimal precio,
            int rarezaId,
            string tipo,
            string? coleccion,
            string? imagenUrl)
        {
            var carta = await _db.Cartas.FindAsync(id);

            if (carta == null)
                return NotFound();

            carta.Nombre = nombre;
            carta.Precio = precio;
            carta.RarezaId = rarezaId;
            carta.Tipo = tipo;
            carta.Coleccion = coleccion;
            carta.ImagenUrl = imagenUrl;

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Carta editada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var carta = await _db.Cartas.FindAsync(id);

            if (carta == null)
                return NotFound();

            _db.Cartas.Remove(carta);

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Carta eliminada correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}
