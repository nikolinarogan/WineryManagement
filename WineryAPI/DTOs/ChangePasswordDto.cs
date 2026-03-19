using System.ComponentModel.DataAnnotations;

namespace WineryAPI.DTOs
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Stara lozinka je obavezna")]
        public string StaraLozinka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova lozinka je obavezna")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Nova lozinka mora imati minimum 6 karaktera")]
        public string NovaLozinka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrda lozinke je obavezna")]
        [Compare("NovaLozinka", ErrorMessage = "Lozinke se ne poklapaju")]
        public string PotvrdaLozinke { get; set; } = string.Empty;
    }
}

