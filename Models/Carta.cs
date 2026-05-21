using System;
using System.Collections.Generic;

namespace MTGRoyal.Models;

public partial class Carta
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string? ImagenUrl { get; set; }

    public decimal Precio { get; set; }

    public string? Coleccion { get; set; }

    public int RarezaId { get; set; }

    public virtual Rareza Rareza { get; set; } = null!;

    public virtual ICollection<Colore> Colors { get; set; } = new List<Colore>();
}
