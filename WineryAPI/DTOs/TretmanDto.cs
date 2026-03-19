namespace WineryAPI.DTOs
{
    public class UbranasirovinaZaTretmanDto
    {
        public int Idubrsir { get; set; }
        public decimal Kolicina { get; set; }
        public string Datprijema { get; set; } = string.Empty;
        public string BerbaNaziv { get; set; } = string.Empty;
        public int Sezona { get; set; }
        public string ParcelaNaziv { get; set; } = string.Empty;
        public string VinogradNaziv { get; set; } = string.Empty;
        public string? SortaNaziv { get; set; }
        public string Status { get; set; } = string.Empty; // Nova/UTretmanu/SpremaZaVino/Obradjena
        public int BrojTretmana { get; set; }
        public int AktivniTretmani { get; set; }
        public bool PostojiSirovovino { get; set; }
        
        public decimal IskoriscenoKolicina { get; set; }  // kg grožđa utrošeno u sirovim vinima
        public decimal PreostalaKolicina { get; set; }    // kg grožđa preostalo
        public int BrojSirovihVina { get; set; }          // Broj kreiranih sirovih vina
    }

    public class TretmanDto
    {
        public int Idtretmana { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Datpocetkatret { get; set; } = string.Empty;
        public string? Datzavresetkatret { get; set; }
        public int TrajanjeUDanima { get; set; }
        public bool JeAktivan { get; set; }
        public int UbranasirovinaIdubrsir { get; set; }
        public int BrojSirovina { get; set; }
        public List<EnologTretmanaDto> Enolozi { get; set; } = new List<EnologTretmanaDto>();
    }

    public class EnologTretmanaDto
    {
        public int Idzap { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prez { get; set; } = string.Empty;
    }

    public class TretmanDetailDto
    {
        public int Idtretmana { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Datpocetkatret { get; set; } = string.Empty;
        public string? Datzavresetkatret { get; set; }
        public int TrajanjeUDanima { get; set; }
        public bool JeAktivan { get; set; }
        public UbranasirovinaInfoDto UbranaSirovina { get; set; } = new UbranasirovinaInfoDto();
        public List<SirovinaUTretmanuDto> Sirovine { get; set; } = new List<SirovinaUTretmanuDto>();
        public List<EnologTretmanaDto> Enolozi { get; set; } = new List<EnologTretmanaDto>();
    }

    public class UbranasirovinaInfoDto
    {
        public int Idubrsir { get; set; }
        public decimal Kolicina { get; set; }
        public string BerbaNaziv { get; set; } = string.Empty;
        public string ParcelaNaziv { get; set; } = string.Empty;
        public string? SortaNaziv { get; set; }
    }

    public class SirovinaUTretmanuDto
    {
        public int SirovinazatretmanIdsir { get; set; }
        public string NazivSirovine { get; set; } = string.Empty;
        public decimal Kolicina { get; set; }
    }

    public class CreateTretmanDto
    {
        public string Naziv { get; set; } = string.Empty;
        public string Datpocetkatret { get; set; } = string.Empty;
        public int UbranasirovinaIdubrsir { get; set; }
    }

    public class CloseTretmanDto
    {
        public string Datzavresetkatret { get; set; } = string.Empty;
    }

    public class AddSirovinaToTretmanDto
    {
        public int SirovinazatretmanIdsir { get; set; }
        public decimal Kolicina { get; set; }
    }
}

