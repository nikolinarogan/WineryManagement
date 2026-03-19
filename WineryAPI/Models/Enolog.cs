using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Enolog
{
    public int Idzap { get; set; }

    public string Brsert { get; set; } = null!;

    public virtual Zaposleni IdzapNavigation { get; set; } = null!;

    public virtual ICollection<Tretman> TretmanIdtretmanas { get; set; } = new List<Tretman>();
}
