using System;
using System.Collections.Generic;

namespace WineryAPI.Models;

public partial class Vinograd
{
    public int Idv { get; set; }

    public string Naziv { get; set; } = null!;

    public DateOnly Datosn { get; set; }

    public decimal Povrsina { get; set; }

    public int Nadmorskavis { get; set; }

    public string Tipzemljista { get; set; } = null!;

    public char Navodnjavanje { get; set; }

    public virtual ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();
}
