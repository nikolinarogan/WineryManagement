namespace WineryAPI.DTOs
{
    public class RadoviDto
    {
        public int Idrad { get; set; }
        public DateOnly Pocrad { get; set; }
        public DateOnly Zavrrad { get; set; }
        public string Oprema { get; set; } = string.Empty;
        public int BrojParcela { get; set; }
        public int BrojRadnika { get; set; }
    }

    public class RadoviDetailDto
    {
        public int Idrad { get; set; }
        public DateOnly Pocrad { get; set; }
        public DateOnly Zavrrad { get; set; }
        public string Oprema { get; set; } = string.Empty;
        public List<ParcelaNaRaduDto> Parcele { get; set; } = new();
        public List<RadnikNaRaduDto> Radnici { get; set; } = new();
    }

    public class ParcelaNaRaduDto
    {
        public int Idp { get; set; }
        public string Nazivparcele { get; set; } = string.Empty;
        public string VinogradNaziv { get; set; } = string.Empty;
        public decimal Povrsina { get; set; }
    }

    public class RadnikNaRaduDto
    {
        public int RadnikIdzap { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class CreateRadoviDto
    {
        public DateOnly Pocrad { get; set; }
        public DateOnly Zavrrad { get; set; }
        public string Oprema { get; set; } = string.Empty;
        public List<int> ParcelaIds { get; set; } = new(); 
    }

    public class UpdateRadoviDto
    {
        public DateOnly Pocrad { get; set; }
        public DateOnly Zavrrad { get; set; }
        public string Oprema { get; set; } = string.Empty;
    }

    public class AddRadnikToRadDto
    {
        public int RadnikIdzap { get; set; }
    }

    public class RadnikRadDto
    {
        public int Idrad { get; set; }
        public DateOnly Pocrad { get; set; }
        public DateOnly Zavrrad { get; set; }
        public string Oprema { get; set; } = string.Empty;
        public List<ParcelaNaRaduDto> Parcele { get; set; } = new();
    }
}

