using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Sirovinazatretman
{
    public int Idsir { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<SeDodaje> SeDodajes { get; set; } = new List<SeDodaje>();
}
