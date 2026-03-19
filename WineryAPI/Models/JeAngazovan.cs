using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class JeAngazovan
{
    public int RasporedbranjaIdras { get; set; }

    public int RadnikIdzap { get; set; }

    public decimal Kolicinaubrgr { get; set; }

    public DateOnly Datumbranja { get; set; }

    public virtual Radnik RadnikIdzapNavigation { get; set; } = null!;

    public virtual Rasporedbranja RasporedbranjaIdrasNavigation { get; set; } = null!;
}
