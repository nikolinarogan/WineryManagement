using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Somleijer
{
    public int Idzap { get; set; }

    public string Specijalnost { get; set; } = null!;

    public virtual Zaposleni IdzapNavigation { get; set; } = null!;

    public virtual ICollection<Degustacija> DegustacijaIddegs { get; set; } = new List<Degustacija>();
}
