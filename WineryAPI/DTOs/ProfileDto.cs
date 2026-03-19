namespace WineryAPI.DTOs
{
    public class ProfileDto
    {
        public int Idzap { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prez { get; set; } = string.Empty;
        public string Jmbg { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Kategorija { get; set; } = string.Empty;

        // Dodatna polja zavisno od kategorije
        public string? Brsert { get; set; }           // Enolog
        public decimal? Bonusucinak { get; set; }      // Menadžer
        public string? Fizickaspremnost { get; set; }  // Radnik
        public string? Specijalnost { get; set; }      // Somleijer
    }
}

