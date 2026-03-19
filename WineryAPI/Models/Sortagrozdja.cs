using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Sortagrozdja
{
    public int Idsrt { get; set; }

    public string Nazivsorte { get; set; } = null!;

    public string Bojasorte { get; set; } = null!;

    public string Porijeklosorte { get; set; } = null!;

    public string Periodsazr { get; set; } = null!;

    public decimal Kiselost { get; set; }

    public virtual ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();

    public virtual ICollection<Ucestvuje> Ucestvujes { get; set; } = new List<Ucestvuje>();
}
