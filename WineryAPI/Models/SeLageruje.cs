using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class SeLageruje
{
    public int SirovovinoIdsirvina { get; set; }

    public int BureIdbur { get; set; }

    public DateOnly Datpunjenja { get; set; }

    public DateOnly Datpraznjenja { get; set; }

    public virtual Bure BureIdburNavigation { get; set; } = null!;

    public virtual Sirovovino SirovovinoIdsirvinaNavigation { get; set; } = null!;
}
