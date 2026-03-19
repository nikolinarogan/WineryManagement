using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Parcela
{
    public int Idp { get; set; }

    public int Brojcokota { get; set; }

    public decimal Povrsina { get; set; }

    public string Nazivparcele { get; set; } = null!;

    public int VinogradIdv { get; set; }

    public int? SortagrozdjaIdsrt { get; set; }

    public virtual ICollection<Seodrzava> Seodrzavas { get; set; } = new List<Seodrzava>();

    public virtual Sortagrozdja? SortagrozdjaIdsrtNavigation { get; set; }

    public virtual Vinograd VinogradIdvNavigation { get; set; } = null!;

    public virtual ICollection<Radovi> RadoviIdrads { get; set; } = new List<Radovi>();
}
