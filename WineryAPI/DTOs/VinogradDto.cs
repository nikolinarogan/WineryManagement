namespace WineryAPI.DTOs
{
    public class VinogradDto
    {
        public int Idv { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public DateOnly Datosn { get; set; }
        public decimal Povrsina { get; set; }
        public int Nadmorskavis { get; set; }
        public string Tipzemljista { get; set; } = string.Empty;
        public char Navodnjavanje { get; set; }
        public int BrojParcela { get; set; }
    }

    public class CreateVinogradDto
    {
        public string Naziv { get; set; } = string.Empty;
        public DateOnly Datosn { get; set; }
        public decimal Povrsina { get; set; }
        public int Nadmorskavis { get; set; }
        public string Tipzemljista { get; set; } = string.Empty;
        public char Navodnjavanje { get; set; }
        public List<CreateParcelaDto>? Parcele { get; set; }
    }

    public class UpdateVinogradDto
    {
        public string Naziv { get; set; } = string.Empty;
        public DateOnly Datosn { get; set; }
        public decimal Povrsina { get; set; }
        public int Nadmorskavis { get; set; }
        public string Tipzemljista { get; set; } = string.Empty;
        public char Navodnjavanje { get; set; }
    }

    public class VinogradDetailDto
    {
        public int Idv { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public DateOnly Datosn { get; set; }
        public decimal Povrsina { get; set; }
        public int Nadmorskavis { get; set; }
        public string Tipzemljista { get; set; } = string.Empty;
        public char Navodnjavanje { get; set; }
        public List<ParcelaDto> Parcele { get; set; } = new();
    }
}

