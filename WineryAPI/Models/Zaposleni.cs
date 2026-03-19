using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Zaposleni
{
    public int Idzap { get; set; }

    public string Ime { get; set; } = null!;

    public string Prez { get; set; } = null!;

    public string Jmbg { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? ZaposleniIdzap { get; set; }

    public string Kategorija { get; set; } = null!;

    public string Sifra { get; set; } = null!;

    public virtual Enolog? Enolog { get; set; }

    public virtual ICollection<Zaposleni> InverseZaposleniIdzapNavigation { get; set; } = new List<Zaposleni>();

    public virtual Menadzer? Menadzer { get; set; }

    public virtual Radnik? Radnik { get; set; }

    public virtual Somleijer? Somleijer { get; set; }

    public virtual Zaposleni? ZaposleniIdzapNavigation { get; set; }
}
