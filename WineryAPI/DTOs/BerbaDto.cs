namespace WineryAPI.DTOs
{
    public class BerbaDto
    {
        public int Idber { get; set; }
        public string Nazber { get; set; } = string.Empty;
        public int Sezona { get; set; }
        public int BrojParcela { get; set; }
    }

    public class BerbaDetailDto
    {
        public int Idber { get; set; }
        public string Nazber { get; set; } = string.Empty;
        public int Sezona { get; set; }
        public List<ParcelaBerbaDto> Parcele { get; set; } = new();
    }

    public class CreateBerbaDto
    {
        public string Nazber { get; set; } = string.Empty;
        public int Sezona { get; set; }
        public List<int> ParcelaIds { get; set; } = new();
    }

    public class UpdateBerbaDto
    {
        public string Nazber { get; set; } = string.Empty;
        public int Sezona { get; set; }
    }

    public class ParcelaBerbaDto
    {
        public int ParcelaIdp { get; set; }
        public string Nazivparcele { get; set; } = string.Empty;
        public string VinogradNaziv { get; set; } = string.Empty;
        public string? SortaNaziv { get; set; }
        public decimal Povrsina { get; set; }
        public int Brojcokota { get; set; }
    }

    public class AddParcelaToBerbaDto
    {
        public int ParcelaId { get; set; }
    }

    // RasporedBranja DTOs
    public class RasporedbranjaDto
    {
        public int Idras { get; set; }
        public DateOnly Pocbranja { get; set; }
        public DateOnly Zavrsetakbranja { get; set; }
        public decimal Ocekivaniprinos { get; set; }
        public int MenadzerIdzap { get; set; }
        public string MenadzerIme { get; set; } = string.Empty;
        public string MenadzerPrezime { get; set; } = string.Empty;
        public int SeodrzavaIdber { get; set; }
        public string BerbaNaziv { get; set; } = string.Empty;
        public int SeodrzavaIdp { get; set; }
        public string ParcelaNaziv { get; set; } = string.Empty;
        public int BrojRadnika { get; set; }
        public int BrojRadnikaSaUnosom { get; set; } // Broj radnika koji je već unio količinu
        public decimal UkupnoUbrano { get; set; } // Ukupna količina koju su radnici unijeli
    }

    public class RasporedbranjaDetailDto
    {
        public int Idras { get; set; }
        public DateOnly Pocbranja { get; set; }
        public DateOnly Zavrsetakbranja { get; set; }
        public decimal Ocekivaniprinos { get; set; }
        public int MenadzerIdzap { get; set; }
        public string MenadzerIme { get; set; } = string.Empty;
        public string MenadzerPrezime { get; set; } = string.Empty;
        public int SeodrzavaIdber { get; set; }
        public string BerbaNaziv { get; set; } = string.Empty;
        public int SeodrzavaIdp { get; set; }
        public string ParcelaNaziv { get; set; } = string.Empty;
        public string VinogradNaziv { get; set; } = string.Empty;
        public List<RadnikNaRasporedDto> Radnici { get; set; } = new();
    }

    public class CreateRasporedbranjaDto
    {
        public DateOnly Pocbranja { get; set; }
        public DateOnly Zavrsetakbranja { get; set; }
        public decimal Ocekivaniprinos { get; set; }
        public int MenadzerIdzap { get; set; }
        public int BerbaIdber { get; set; }
        public int ParcelaIdp { get; set; }
    }

    public class UpdateRasporedbranjaDto
    {
        public DateOnly Pocbranja { get; set; }
        public DateOnly Zavrsetakbranja { get; set; }
        public decimal Ocekivaniprinos { get; set; }
    }

    public class RadnikNaRasporedDto
    {
        public int RadnikIdzap { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public decimal Kolicinaubrgr { get; set; }
        public DateOnly Datumbranja { get; set; }
    }

    public class AddRadnikToRasporedDto
    {
        public int RadnikIdzap { get; set; }
        public decimal Kolicinaubrgr { get; set; }
        public DateOnly Datumbranja { get; set; }
    }

    public class UpdateRadnikKolicinaDto
    {
        public decimal Kolicinaubrgr { get; set; }
        public DateOnly Datumbranja { get; set; }
    }

    public class RadnikRasporedDto
    {
        public int Idras { get; set; }
        public string BerbaNaziv { get; set; } = string.Empty;
        public string ParcelaNaziv { get; set; } = string.Empty;
        public string VinogradNaziv { get; set; } = string.Empty;
        public DateOnly Pocbranja { get; set; }
        public DateOnly Zavrsetakbranja { get; set; }
        public decimal MojaKolicina { get; set; }
        public DateOnly? MojDatumBranja { get; set; }
        public string MenadzerIme { get; set; } = string.Empty;
        public string MenadzerPrezime { get; set; } = string.Empty;
    }

    public class BerbaStatistikaDto
    {
        public int BerbaIdber { get; set; }
        public string BerbaNaziv { get; set; } = string.Empty;
        public int Sezona { get; set; }
        public decimal UkupnoUbrano { get; set; }
        public decimal OcekivanoUkupno { get; set; }
        public decimal ProcenatRealizacije { get; set; }
        public int BrojRadnika { get; set; }
        public int BrojRasporeda { get; set; }
        public List<RadnikUcinakDto> RadniciUcinak { get; set; } = new();
    }

    public class RadnikUcinakDto
    {
        public int RadnikId { get; set; }
        public string RadnikIme { get; set; } = string.Empty;
        public string RadnikPrezime { get; set; } = string.Empty;
        public decimal Kolicina { get; set; }
        public DateOnly? DatumBranja { get; set; }
        public string ParcelaNaziv { get; set; } = string.Empty;
    }
}

