using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Magacin
{
    public int Idmag { get; set; }

    public string Nazivmag { get; set; } = null!;

    public int Kapacitetmag { get; set; }

    public decimal Tempmag { get; set; }

    public virtual ICollection<Boca> Bocas { get; set; } = new List<Boca>();
}
