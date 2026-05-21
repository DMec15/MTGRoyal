using System;
using System.Collections.Generic;

namespace MTGRoyal.Models;

public partial class Rareza
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Carta> Carta { get; set; } = new List<Carta>();
}
