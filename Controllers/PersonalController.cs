using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTGRoyal.Models;
using MTGRoyal.Services;

namespace MTGRoyal.Controllers
{
    public class PersonalController : Controller
    {
        private readonly MtgroyalDbContext _db;
        private readonly TemporaryStateService temporaryState;

        public PersonalController(
            MtgroyalDbContext db,
            TemporaryStateService temporaryState)
        {
            _db = db;
            this.temporaryState = temporaryState;
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
            await LoadAdminCards();

            var draft = temporaryState.GetPersonalDraft();

            if (HasPersonalDraft(draft))
            {
                ViewBag.Form = new CartaCreateRequest
                {
                    Nombre = draft.Nombre,
                    Precio = draft.Precio,
                    RarezaId = draft.RarezaId,
                    Tipo = draft.Tipo,
                    Coleccion = draft.Coleccion,
                    ImagenUrl = draft.ImagenUrl,
                    ColorIds = draft.ColorIds
                };
            }


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

            if (string.IsNullOrWhiteSpace(request.Coleccion))
                ModelState.AddModelError(nameof(request.Coleccion), "La coleccion es obligatoria.");

            if (string.IsNullOrWhiteSpace(request.ImagenUrl))
                ModelState.AddModelError(nameof(request.ImagenUrl), "La imagen es obligatoria.");

            if (request.ColorIds.Length == 0)
                ModelState.AddModelError(nameof(request.ColorIds), "Selecciona al menos un color.");

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
                await LoadAdminCards();
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
            temporaryState.ClearPersonalDraft();

            TempData["SuccessMessage"] = "Carta agregada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult GuardarBorrador([FromBody] PersonalDraftState draft)
        {
            temporaryState.SavePersonalDraft(draft);

            return Ok();
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

        private async Task LoadAdminCards()
        {
            ViewBag.Cartas = await _db.Cartas
                .Include(c => c.Rareza)
                .Include(c => c.Colors)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        private static bool HasPersonalDraft(PersonalDraftState draft)
        {
            return !string.IsNullOrWhiteSpace(draft.Nombre) ||
                draft.Precio > 0 ||
                draft.RarezaId > 0 ||
                !string.IsNullOrWhiteSpace(draft.Tipo) ||
                !string.IsNullOrWhiteSpace(draft.Coleccion) ||
                !string.IsNullOrWhiteSpace(draft.ImagenUrl) ||
                draft.ColorIds.Length > 0;
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
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(tipo) ||
                string.IsNullOrWhiteSpace(coleccion) ||
                string.IsNullOrWhiteSpace(imagenUrl) ||
                rarezaId <= 0 ||
                precio < 0)
            {
                TempData["SuccessMessage"] =
                    "No se pudo editar la carta: faltan datos obligatorios.";

                return RedirectToAction(nameof(Index));
            }

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
