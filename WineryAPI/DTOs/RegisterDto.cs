using System.ComponentModel.DataAnnotations;

namespace WineryAPI.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ime je obavezno")]
        [StringLength(20, ErrorMessage = "Ime ne može biti duže od 20 karaktera")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno")]
        [StringLength(30, ErrorMessage = "Prezime ne može biti duže od 30 karaktera")]
        public string Prez { get; set; } = string.Empty;

        [Required(ErrorMessage = "JMBG je obavezan")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG mora imati tačno 13 cifara")]
        public string Jmbg { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese")]
        [StringLength(100, ErrorMessage = "Email ne može biti duži od 100 karaktera")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategorija je obavezna")]
        public string Kategorija { get; set; } = string.Empty; // "Enolog", "Somleijer", "Radnik"

        [Required(ErrorMessage = "Privremena lozinka je obavezna")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Lozinka mora imati minimum 6 karaktera")]
        public string PrivremenaLozinka { get; set; } = string.Empty;

        public int? SuperiorId { get; set; } // ID menadžera koji kreira radnika

        // Dodatna polja za Enologa
        public string? Brsert { get; set; } // Broj sertifikata

        // Dodatna polja za Radnika
        public string? Fizickaspremnost { get; set; } // Fizička spremnost

        // Dodatna polja za Somleijera
        public string? Specijalnost { get; set; } // Specijalnost
    }
}

