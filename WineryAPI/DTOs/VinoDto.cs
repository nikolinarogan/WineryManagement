namespace WineryAPI.DTOs
{
    public class VinoDto
    {
        public int Idvina { get; set; }
        public string Nazivvina { get; set; } = string.Empty;
        public decimal Procalk { get; set; }
        public string Tipvina { get; set; } = string.Empty;
        public int BrojSirovihVina { get; set; }
        public List<SirovoVinoUVinuDto> SirovaVina { get; set; } = new List<SirovoVinoUVinuDto>();
    }

    public class SirovoVinoUVinuDto
    {
        public int Idsirvina { get; set; }
        public string Nazivsirvina { get; set; } = string.Empty;
        public decimal Kolicinasirvina { get; set; }
        public string Kvalitet { get; set; } = string.Empty;
        public int Godproizvodnje { get; set; }
    }

    public class VinoDetailDto
    {
        public int Idvina { get; set; }
        public string Nazivvina { get; set; } = string.Empty;
        public decimal Procalk { get; set; }
        public string Tipvina { get; set; } = string.Empty;
        public List<SirovoVinoUVinuDto> SirovaVina { get; set; } = new List<SirovoVinoUVinuDto>();
    }

    public class CreateVinoDto
    {
        public string Nazivvina { get; set; } = string.Empty;
        public decimal Procalk { get; set; }
        public string Tipvina { get; set; } = string.Empty;
        public List<int> SirovaVinaIds { get; set; } = new List<int>();
    }

    public class UpdateVinoDto
    {
        public string Nazivvina { get; set; } = string.Empty;
        public decimal Procalk { get; set; }
        public string Tipvina { get; set; } = string.Empty;
    }
}

