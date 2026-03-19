using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class SeDodaje
{
    public int SirovinazatretmanIdsir { get; set; }

    public int TretmanIdtretmana { get; set; }

    public decimal Dodatakolicina { get; set; }

    public virtual Sirovinazatretman SirovinazatretmanIdsirNavigation { get; set; } = null!;

    public virtual Tretman TretmanIdtretmanaNavigation { get; set; } = null!;
}
