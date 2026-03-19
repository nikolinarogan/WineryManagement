using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Seodrzava
{
    public int BerbaIdber { get; set; }

    public int ParcelaIdp { get; set; }

    public virtual Berba BerbaIdberNavigation { get; set; } = null!;

    public virtual Parcela ParcelaIdpNavigation { get; set; } = null!;

    public virtual ICollection<Rasporedbranja> Rasporedbranjas { get; set; } = new List<Rasporedbranja>();
}
