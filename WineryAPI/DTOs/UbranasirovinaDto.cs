namespace WineryAPI.DTOs
{
    public class UbranasirovinaDto
    {
        public int Idubrsir { get; set; }
        public decimal Kolicina { get; set; }
        public DateOnly Datprijema { get; set; }
        public int RasporedbranjaIdras { get; set; }
        public string BerbaNaziv { get; set; } = string.Empty;
        public string ParcelaNaziv { get; set; } = string.Empty;
        public string VinogradNaziv { get; set; } = string.Empty;
        public string? SortaNaziv { get; set; }
    }

    public class CreateUbranasirovinaDto
    {
        public decimal Kolicina { get; set; }
        public DateOnly Datprijema { get; set; }
        public int RasporedbranjaIdras { get; set; }
    }

    public class UpdateUbranasirovinaDto
    {
        public decimal Kolicina { get; set; }
        public DateOnly Datprijema { get; set; }
    }

    public class RasporedZaPrijemDto
    {
        public int RasporedId { get; set; }
        public string BerbaNaziv { get; set; } = string.Empty;
        public int Sezona { get; set; }
        public string ParcelaNaziv { get; set; } = string.Empty;
        public string VinogradNaziv { get; set; } = string.Empty;
        public string? SortaNaziv { get; set; }
        public DateOnly PocetakBranja { get; set; }
        public DateOnly ZavrsetakBranja { get; set; }
        public decimal OcekivaniPrinos { get; set; }
        public decimal UkupnoUbrano { get; set; }
        public decimal ProcenatRealizacije { get; set; }
        public int UkupnoRadnika { get; set; }
        public int RadnikaSaUnosom { get; set; }
    }

    public class PrijemDetailsDto
    {
        public int RasporedId { get; set; }
        public string BerbaNaziv { get; set; } = string.Empty;
        public string ParcelaNaziv { get; set; } = string.Empty;
        public string VinogradNaziv { get; set; } = string.Empty;
        public string? SortaNaziv { get; set; }
        public DateOnly PocetakBranja { get; set; }
        public DateOnly ZavrsetakBranja { get; set; }
        
        // Planirano
        public decimal OcekivaniPrinos { get; set; }
        
        // Realizacija
        public decimal UkupnoUbrano { get; set; }
        public decimal ProcenatRealizacije { get; set; }
        public decimal ProsekPoRadniku { get; set; }
        public decimal StandardnaDevijacija { get; set; }
        
        // Radnici sa količinama
        public List<RadnikKolicinaDto> Radnici { get; set; } = new();
        
        // Anomalije
        public int BrojAnomalija { get; set; }
        public List<RadnikAnomalijaDto> Anomalije { get; set; } = new();
        
        // Preporuke za prijem
        public decimal PreporucenaKolicina { get; set; } // 97% od ubranog
        public decimal MinValidnaKolicina { get; set; } // 85% od ubranog
        public decimal MaxValidnaKolicina { get; set; } // 100% od ubranog
    }

    public class RadnikKolicinaDto
    {
        public int RadnikId { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public decimal Kolicina { get; set; }
        public DateOnly Datum { get; set; }
        public decimal OdstupostotakOdProseka { get; set; }
        public bool IsOutlier { get; set; }
    }

    public class RadnikAnomalijaDto
    {
        public int RadnikId { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public decimal Kolicina { get; set; }
        public decimal Prosek { get; set; }
        public decimal OdstupostotakPostotak { get; set; }
        public string Tip { get; set; } = string.Empty; // IZNAD_PROSEKA, ISPOD_PROSEKA
        public string Poruka { get; set; } = string.Empty;
    }

    public class PrijemValidationResultDto
    {
        public bool Valid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public decimal UkupnoUbrano { get; set; }
        public decimal Gubici { get; set; }
        public decimal PostotakGubitaka { get; set; }
    }
}

