namespace WineryAPI.DTOs
{
    public class MagacinDto
    {
        public int Idmag { get; set; }
        public string Nazivmag { get; set; } = string.Empty;
        public int Kapacitetmag { get; set; }
        public decimal Tempmag { get; set; }
        public int BrojBoca { get; set; }
        public int Popunjenost { get; set; } // Procenat popunjenosti
    }

    public class MagacinDetailDto
    {
        public int Idmag { get; set; }
        public string Nazivmag { get; set; } = string.Empty;
        public int Kapacitetmag { get; set; }
        public decimal Tempmag { get; set; }
        public int BrojBoca { get; set; }
        public int Popunjenost { get; set; }
    }

    public class CreateMagacinDto
    {
        public string Nazivmag { get; set; } = string.Empty;
        public int Kapacitetmag { get; set; }
        public decimal Tempmag { get; set; }
    }

    public class UpdateMagacinDto
    {
        public string Nazivmag { get; set; } = string.Empty;
        public int Kapacitetmag { get; set; }
        public decimal Tempmag { get; set; }
    }
}

