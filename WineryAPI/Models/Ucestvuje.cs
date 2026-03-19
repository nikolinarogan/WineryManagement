using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Ucestvuje
{
    public int SortagrozdjaIdsrt { get; set; }

    public int VinoIdvina { get; set; }

    public decimal Procenat { get; set; }

    public virtual Sortagrozdja SortagrozdjaIdsrtNavigation { get; set; } = null!;

    public virtual Vino VinoIdvinaNavigation { get; set; } = null!;
}
