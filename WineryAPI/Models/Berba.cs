using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Berba
{
    public int Idber { get; set; }

    public string Nazber { get; set; } = null!;

    public int Sezona { get; set; }

    public virtual ICollection<Seodrzava> Seodrzavas { get; set; } = new List<Seodrzava>();
}
