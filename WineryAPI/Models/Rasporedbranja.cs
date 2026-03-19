using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Rasporedbranja
{
    public int Idras { get; set; }

    public DateOnly Pocbranja { get; set; }

    public DateOnly Zavrsetakbranja { get; set; }

    public decimal Ocekivaniprinos { get; set; }

    public int MenadzerIdzap { get; set; }

    public int SeodrzavaIdber { get; set; }

    public int SeodrzavaIdp { get; set; }

    public virtual ICollection<JeAngazovan> JeAngazovans { get; set; } = new List<JeAngazovan>();

    public virtual Menadzer MenadzerIdzapNavigation { get; set; } = null!;

    public virtual Seodrzava Seodrzava { get; set; } = null!;

    public virtual Ubranasirovina? Ubranasirovina { get; set; }
}
