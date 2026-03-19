using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Tretman
{
    public int Idtretmana { get; set; }

    public string Naziv { get; set; } = null!;

    public DateOnly Datpocetkatret { get; set; }

    public DateOnly? Datzavresetkatret { get; set; }

    public int UbranasirovinaIdubrsir { get; set; }

    public virtual ICollection<SeDodaje> SeDodajes { get; set; } = new List<SeDodaje>();

    public virtual Ubranasirovina UbranasirovinaIdubrsirNavigation { get; set; } = null!;

    public virtual ICollection<Enolog> EnologIdzaps { get; set; } = new List<Enolog>();
}
