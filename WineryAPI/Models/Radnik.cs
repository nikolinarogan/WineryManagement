using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Radnik
{
    public int Idzap { get; set; }

    public string Fizickaspremnost { get; set; } = null!;

    public virtual Zaposleni IdzapNavigation { get; set; } = null!;

    public virtual ICollection<JeAngazovan> JeAngazovans { get; set; } = new List<JeAngazovan>();

    public virtual ICollection<Radovi> RadoviIdrads { get; set; } = new List<Radovi>();
}
