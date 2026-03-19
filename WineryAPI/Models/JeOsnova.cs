using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class JeOsnova
{
    public int UbranasirovinaIdubrsir { get; set; }

    public int SirovovinoIdsirvina { get; set; }

    public decimal KolicinaGrozdja { get; set; }

    public virtual Ubranasirovina UbranasirovinaIdubrsirNavigation { get; set; } = null!;

    public virtual Sirovovino SirovovinoIdsirvinaNavigation { get; set; } = null!;
}

