using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Boca
{
    public int Idboce { get; set; }

    public float? Cena { get; set; }

    public float Zapremina { get; set; }

    public int? VinoIdvina { get; set; }

    public int? MagacinIdmag { get; set; }

    public virtual Magacin? MagacinIdmagNavigation { get; set; }

    public virtual Vino? VinoIdvinaNavigation { get; set; }
}
