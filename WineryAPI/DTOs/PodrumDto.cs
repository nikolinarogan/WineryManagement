namespace WineryAPI.DTOs
{
    public class PodrumDto
    {
        public int Idpod { get; set; }
        public decimal Temp { get; set; }
        public string Nazivpod { get; set; } = string.Empty;
        public int BrojBuradi { get; set; }
    }

    public class PodrumDetailDto
    {
        public int Idpod { get; set; }
        public decimal Temp { get; set; }
        public string Nazivpod { get; set; } = string.Empty;
        public int BrojBuradi { get; set; }
    }

    public class CreatePodrumDto
    {
        public decimal Temp { get; set; }
        public string Nazivpod { get; set; } = string.Empty;
    }

    public class UpdatePodrumDto
    {
        public decimal Temp { get; set; }
        public string Nazivpod { get; set; } = string.Empty;
    }
}

