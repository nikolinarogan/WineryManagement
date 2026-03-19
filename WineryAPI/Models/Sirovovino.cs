using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Sirovovino
{
    public int Idsirvina { get; set; }

    public decimal Kolicinasirvina { get; set; }

    public string Kvalitet { get; set; } = null!;

    public string Nazivsirvina { get; set; } = null!;

    public int Godproizvodnje { get; set; }

    public virtual ICollection<SeLageruje> SeLagerujes { get; set; } = new List<SeLageruje>();

    // STARA implicitna Many-to-Many veza (zakomentarisano):
    // public virtual ICollection<Ubranasirovina> UbranasirovinaIdubrsirs { get; set; } = new List<Ubranasirovina>();

    // NOVA eksplicitna veza preko JeOsnova:
    public virtual ICollection<JeOsnova> JeOsnovas { get; set; } = new List<JeOsnova>();

    public virtual ICollection<Vino> VinoIdvinas { get; set; } = new List<Vino>();
}
