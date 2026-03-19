using System.ComponentModel.DataAnnotations;

namespace WineryAPI.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna")]
        public string Sifra { get; set; } = string.Empty;
    }
}

