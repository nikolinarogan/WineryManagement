using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Podrum
{
    public int Idpod { get; set; }

    public decimal Temp { get; set; }

    public string Nazivpod { get; set; } = null!;

    public virtual ICollection<Bure> Bures { get; set; } = new List<Bure>();
}
