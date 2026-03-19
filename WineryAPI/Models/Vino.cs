using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Vino
{
    public int Idvina { get; set; }

    public string Nazivvina { get; set; } = null!;

    public decimal Procalk { get; set; }

    public string Tipvina { get; set; } = null!;

    public virtual ICollection<Boca> Bocas { get; set; } = new List<Boca>();

    public virtual ICollection<Ucestvuje> Ucestvujes { get; set; } = new List<Ucestvuje>();

    public virtual ICollection<Degustacija> DegustacijaIddegs { get; set; } = new List<Degustacija>();

    public virtual ICollection<Sirovovino> SirovovinoIdsirvinas { get; set; } = new List<Sirovovino>();
}
