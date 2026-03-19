namespace WineryAPI.DTOs
{
    public class BureDto
    {
        public int Idbur { get; set; }
        public decimal Zapremina { get; set; }
        public string Materijal { get; set; } = string.Empty;
        public string Oznakabur { get; set; } = string.Empty;
        public int? PodrumIdpod { get; set; }
        public string? PodrumNaziv { get; set; }
    }

    public class BureDetailDto
    {
        public int Idbur { get; set; }
        public decimal Zapremina { get; set; }
        public string Materijal { get; set; } = string.Empty;
        public string Oznakabur { get; set; } = string.Empty;
        public int? PodrumIdpod { get; set; }
        public string? PodrumNaziv { get; set; }
        public int BrojLagerovanihVina { get; set; }
    }

    public class CreateBureDto
    {
        public decimal Zapremina { get; set; }
        public string Materijal { get; set; } = string.Empty;
        public string Oznakabur { get; set; } = string.Empty;
        public int PodrumIdpod { get; set; }
    }

    public class UpdateBureDto
    {
        public decimal Zapremina { get; set; }
        public string Materijal { get; set; } = string.Empty;
        public string Oznakabur { get; set; } = string.Empty;
    }
}

