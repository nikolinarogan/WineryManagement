using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Ubranasirovina
{
    public int Idubrsir { get; set; }

    public decimal Kolicina { get; set; }

    public DateOnly Datprijema { get; set; }

    public int RasporedbranjaIdras { get; set; }

    public virtual Rasporedbranja RasporedbranjaIdrasNavigation { get; set; } = null!;

    public virtual ICollection<Tretman> Tretmen { get; set; } = new List<Tretman>();

    // STARA implicitna Many-to-Many veza (zakomentarisano):
    // public virtual ICollection<Sirovovino> SirovovinoIdsirvinas { get; set; } = new List<Sirovovino>();

    // NOVA eksplicitna veza preko JeOsnova:
    public virtual ICollection<JeOsnova> JeOsnovas { get; set; } = new List<JeOsnova>();
}
