using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Degustacija
{
    public int Iddeg { get; set; }

    public string Nazivdeg { get; set; } = null!;

    public DateOnly Datdeg { get; set; }

    public int Kapacitetdeg { get; set; }

    public virtual ICollection<Somleijer> SomleijerIdzaps { get; set; } = new List<Somleijer>();

    public virtual ICollection<Vino> VinoIdvinas { get; set; } = new List<Vino>();
}
