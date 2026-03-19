namespace WineryAPI.DTOs
{
    public class DegustacijaDto
    {
        public int Iddeg { get; set; }
        public string Nazivdeg { get; set; } = string.Empty;
        public DateOnly Datdeg { get; set; }
        public int Kapacitetdeg { get; set; }
        public List<VinoBasicDto> Vina { get; set; } = new List<VinoBasicDto>();
        public List<SomleijerBasicDto> Somelijeri { get; set; } = new List<SomleijerBasicDto>();
    }

    public class DegustacijaBasicDto
    {
        public int Iddeg { get; set; }
        public string Nazivdeg { get; set; } = string.Empty;
        public DateOnly Datdeg { get; set; }
        public int Kapacitetdeg { get; set; }
        public int BrojVina { get; set; }
    }

    public class CreateDegustacijaDto
    {
        public string Nazivdeg { get; set; } = string.Empty;
        public DateOnly Datdeg { get; set; }
        public int Kapacitetdeg { get; set; }
        public List<int> VinaIds { get; set; } = new List<int>(); // IDs finalnih vina
        public int SomleijerIdzap { get; set; } // ID somelijera koji kreira
    }

    public class UpdateDegustacijaDto
    {
        public string Nazivdeg { get; set; } = string.Empty;
        public DateOnly Datdeg { get; set; }
        public int Kapacitetdeg { get; set; }
        public List<int> VinaIds { get; set; } = new List<int>();
    }

    public class VinoBasicDto
    {
        public int Idvina { get; set; }
        public string Nazivvina { get; set; } = string.Empty;
        public string Tipvina { get; set; } = string.Empty;
    }

    public class SomleijerBasicDto
    {
        public int Idzap { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Specijalnost { get; set; } = string.Empty;
    }
}

