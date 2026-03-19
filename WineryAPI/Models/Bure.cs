using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Bure
{
    public int Idbur { get; set; }

    public decimal Zapremina { get; set; }

    public string Materijal { get; set; } = null!;

    public string Oznakabur { get; set; } = null!;

    public int? PodrumIdpod { get; set; }

    public virtual Podrum? PodrumIdpodNavigation { get; set; }

    public virtual ICollection<SeLageruje> SeLagerujes { get; set; } = new List<SeLageruje>();
}
