namespace WineryAPI.DTOs
{
    public class BocaDto
    {
        public int Idboce { get; set; }
        public float? Cena { get; set; }
        public float Zapremina { get; set; }
        public int? VinoIdvina { get; set; }
        public string NazivVina { get; set; } = string.Empty;
        public string TipVina { get; set; } = string.Empty;
        public int? MagacinIdmag { get; set; }
        public string NazivMagacina { get; set; } = string.Empty;
    }

    public class BocaInventarDto
    {
        public int Idboce { get; set; }
        public float? Cena { get; set; }
        public float Zapremina { get; set; }
        public string NazivVina { get; set; } = string.Empty;
        public string TipVina { get; set; } = string.Empty;
        public string NazivMagacina { get; set; } = string.Empty;
    }

    public class CreateBocaPunjenjeDto
    {
        public int VinoIdvina { get; set; }          // Finalno vino
        public int MagacinIdmag { get; set; }        // Magacin za skladištenje
        public int BrojBoca { get; set; }            // Koliko boca da kreira
        public float Zapremina { get; set; }         // Zapremina boce (0.75L, itd.)
        public float? Cena { get; set; }             // Cena (opciono)
    }

    public class BocePunjenjeResultDto
    {
        public int BrojKreiranihBoca { get; set; }
        public string NazivVina { get; set; } = string.Empty;
        public string NazivMagacina { get; set; } = string.Empty;
        public List<int> KreiraneBoceIds { get; set; } = new List<int>();
    }
}

