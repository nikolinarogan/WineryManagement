namespace WineryAPI.DTOs
{
    public class SirovovinoDto
    {
        public int Idsirvina { get; set; }
        public string Nazivsirvina { get; set; } = string.Empty;
        public decimal Kolicinasirvina { get; set; }
        public string Kvalitet { get; set; } = string.Empty;
        public int Godproizvodnje { get; set; }
        public List<PorekloSirovinaDto> Poreklo { get; set; } = new List<PorekloSirovinaDto>();
    }

    public class PorekloSirovinaDto
    {
        public int UbranasirovinaId { get; set; }
        public string BerbaNaziv { get; set; } = string.Empty;
        public string ParcelaNaziv { get; set; } = string.Empty;
        public string? SortaNaziv { get; set; }
        public int BrojTretmana { get; set; }
        public decimal KolicinaGrozdja { get; set; }  
    }

    public class UbranaSirovinaInputDto
    {
        public int UbranasirovinaId { get; set; }
        public decimal KolicinaGrozdja { get; set; }  // kg grožđa koje se koristi
    }

    public class CreateSirovovinoDto
    {
        public string Nazivsirvina { get; set; } = string.Empty;
        public decimal Kolicinasirvina { get; set; }  // Litara vina
        public string Kvalitet { get; set; } = string.Empty;
        public int Godproizvodnje { get; set; }
        
        // Lista ubranih sirovina sa količinom grožđa (omogućava i blending)
        public List<UbranaSirovinaInputDto> UbraneSirovine { get; set; } = new List<UbranaSirovinaInputDto>();
    }

    public class UpdateSirovovinoDto
    {
        public string Nazivsirvina { get; set; } = string.Empty;
        public decimal Kolicinasirvina { get; set; }
        public string Kvalitet { get; set; } = string.Empty;
    }
}

