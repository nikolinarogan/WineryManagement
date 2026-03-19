using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Radovi
{
    public int Idrad { get; set; }

    public DateOnly Pocrad { get; set; }

    public DateOnly Zavrrad { get; set; }

    public string Oprema { get; set; } = null!;

    public virtual ICollection<Parcela> ParcelaIdps { get; set; } = new List<Parcela>();

    public virtual ICollection<Radnik> RadnikIdzaps { get; set; } = new List<Radnik>();
}
