namespace WineryAPI.DTOs
{
    public class SeLagerujeDto
    {
        public int SirovovinoIdsirvina { get; set; }
        public int BureIdbur { get; set; }
        public DateOnly Datpunjenja { get; set; }
        public DateOnly Datpraznjenja { get; set; }
        public string NazivSirovogVina { get; set; } = string.Empty;
        public string OznakaBureta { get; set; } = string.Empty;
        public string NazivPodruma { get; set; } = string.Empty;
    }

    public class LagerovanoVinoDto
    {
        public int SirovovinoIdsirvina { get; set; }
        public string NazivSirovogVina { get; set; } = string.Empty;
        public int BureIdbur { get; set; }
        public string OznakaBureta { get; set; } = string.Empty;
        public string MaterijalBureta { get; set; } = string.Empty;
        public decimal ZapreminaBureta { get; set; }
        public string NazivPodruma { get; set; } = string.Empty;
        public DateOnly Datpunjenja { get; set; }
        public DateOnly Datpraznjenja { get; set; }
        public bool JeAktivno { get; set; } // Da li je vino trenutno u buretu
        public int BrojDana { get; set; } // Broj dana lagerovanja
    }

    public class DostupnoBureDto
    {
        public int Idbur { get; set; }
        public string Oznakabur { get; set; } = string.Empty;
        public string Materijal { get; set; } = string.Empty;
        public decimal Zapremina { get; set; }
        public string NazivPodruma { get; set; } = string.Empty;
        public bool JeZauzeto { get; set; }
    }

    public class CreateLagerovanjeDto
    {
        public int SirovovinoIdsirvina { get; set; }
        public int BureIdbur { get; set; }
        public DateOnly Datpunjenja { get; set; }
    }

    public class UpdateLagerovanjeDto
    {
        public DateOnly Datpraznjenja { get; set; }
    }
}

