using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Menadzer
{
    public int Idzap { get; set; }

    public decimal Bonusucinak { get; set; }

    public virtual Zaposleni IdzapNavigation { get; set; } = null!;

    public virtual ICollection<Rasporedbranja> Rasporedbranjas { get; set; } = new List<Rasporedbranja>();
}
